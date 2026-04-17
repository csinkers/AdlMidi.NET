using System;
using System.Runtime.InteropServices;

namespace ADLMidi.NET;

/// <summary>
/// Thin wrapper around a libadlmidi <c>ADL_MIDIPlayer*</c> configured for raw
/// OPL register writes. Intended for use cases that synthesize their own OPL
/// register streams (Ultima Underworld TVFX effects, DRO playback, custom
/// state machines).
///
/// Backed by two additions to libadlmidi's public API (feat/barechip-wrapper
/// branch on abedegno/libADLMIDI):
///
/// <list type="bullet">
///  <item><c>adl_rt_rawOPL3(device, chipId, reg, val)</c> — writes one
///        OPL3 register on a specific chip, reusing the same internal path
///        that IMF/KLM file playback uses.</item>
///  <item><c>adl_reserveChipChannels(device, chipId, mask)</c> — marks chip
///        channels as off-limits to the MIDI voice allocator so music
///        playback and raw SFX writes can coexist on the same chip.</item>
/// </list>
///
/// When constructed for single-chip raw-only use (the common case for UW
/// SFX), this class reserves all 18 melodic channels + 5 rhythm channels so
/// the MIDI driver never touches the chip. To coexist with MIDI on the same
/// chip, use <see cref="FromPlayer"/> passing only the per-chip channel
/// bitmask you want to own.
/// </summary>
public sealed class OplChip : IDisposable
{
    private const int PerChipChannels = 23; // matches libadlmidi NUM_OF_CHANNELS
    /// <summary>Mask reserving all OPL3 chip-level channels (18 melodic + 5 rhythm).</summary>
    public const uint AllChannelsMask = (1u << PerChipChannels) - 1u;

    private readonly IntPtr _device;
    private readonly int _chipId;
    private readonly bool _ownsDevice;
    private bool _disposed;

    private OplChip(IntPtr device, int chipId, bool ownsDevice)
    {
        _device = device;
        _chipId = chipId;
        _ownsDevice = ownsDevice;
    }

    /// <summary>
    /// Create a stand-alone chip: allocates its own <c>ADL_MIDIPlayer</c>,
    /// configures a single OPL3 chip at <paramref name="sampleRateHz"/>, and
    /// reserves all chip channels so the MIDI driver never allocates voices.
    /// The returned instance owns the underlying device and disposes it.
    /// </summary>
    public static OplChip Create(int sampleRateHz)
    {
        if (sampleRateHz <= 0) throw new ArgumentOutOfRangeException(nameof(sampleRateHz));

        IntPtr device = AdlMidiImports.adl_init(sampleRateHz);
        if (device == IntPtr.Zero)
            throw new InvalidOperationException("adl_init returned null");

        try
        {
            int rc = AdlMidiImports.adl_setNumChips(device, 1);
            if (rc < 0)
                throw new InvalidOperationException($"adl_setNumChips(1) failed: {rc}");

            // Reserve every per-chip channel so the MIDI driver will not
            // allocate any voice on this chip.
            int rr = Native.adl_reserveChipChannels(device, 0, AllChannelsMask);
            if (rr != 0)
                throw new InvalidOperationException($"adl_reserveChipChannels failed: {rr}");

            return new OplChip(device, 0, ownsDevice: true);
        }
        catch
        {
            AdlMidiImports.adl_close(device);
            throw;
        }
    }

    /// <summary>
    /// Wrap an existing <c>ADL_MIDIPlayer*</c> (e.g. one already playing MIDI)
    /// so raw writes can target a specific chip in its emulator farm. Reserves
    /// the given per-chip channel bitmask from the MIDI allocator. Does not
    /// take ownership of the device.
    /// </summary>
    /// <param name="device">Native handle from <c>adl_init</c>.</param>
    /// <param name="chipId">Zero-based chip index.</param>
    /// <param name="reserveMask">Per-chip channel bitmask to reserve (see
    ///   <see cref="AllChannelsMask"/>); 0 to not reserve anything.</param>
    public static OplChip FromPlayer(IntPtr device, int chipId, uint reserveMask)
    {
        if (device == IntPtr.Zero) throw new ArgumentNullException(nameof(device));
        if (chipId < 0) throw new ArgumentOutOfRangeException(nameof(chipId));

        if (reserveMask != 0)
        {
            int rr = Native.adl_reserveChipChannels(device, chipId, reserveMask);
            if (rr != 0)
                throw new InvalidOperationException($"adl_reserveChipChannels failed: {rr}");
        }
        return new OplChip(device, chipId, ownsDevice: false);
    }

    /// <summary>Write <paramref name="val"/> to OPL register <paramref name="addr"/>.</summary>
    public void WriteReg(int addr, byte val)
    {
        ThrowIfDisposed();
        int rc = Native.adl_rt_rawOPL3(_device, _chipId, (ushort)addr, val);
        if (rc == 0)
            throw new InvalidOperationException("adl_rt_rawOPL3 returned 0 (invalid chipId?)");
    }

    /// <summary>
    /// Render <paramref name="frames"/> stereo frames into
    /// <paramref name="interleavedStereo"/> (length >= 2 * frames). Mixes
    /// output from every chip in the device — which, for a stand-alone
    /// <see cref="Create"/> instance, is just the one chip.
    /// </summary>
    public unsafe void GenerateFrames(short[] interleavedStereo, int frames)
    {
        ThrowIfDisposed();
        if (interleavedStereo == null) throw new ArgumentNullException(nameof(interleavedStereo));
        if (frames < 0) throw new ArgumentOutOfRangeException(nameof(frames));
        if (interleavedStereo.Length < frames * 2)
            throw new ArgumentException("buffer smaller than 2 * frames", nameof(interleavedStereo));
        if (frames == 0) return;

        fixed (short* ptr = interleavedStereo)
        {
            // adl_generate takes a total interleaved sample count (2 * frames
            // for stereo), not a frame count.
            AdlMidiImports.adl_generate(_device, frames * 2, ptr);
        }
    }

    /// <summary>
    /// Reset the chip to its power-on state via <c>adl_reset</c>. Only valid
    /// for instances created with <see cref="Create"/> (owners of the device);
    /// otherwise throws, because resetting a shared device would disrupt MIDI
    /// playback.
    /// </summary>
    public void Reset()
    {
        ThrowIfDisposed();
        if (!_ownsDevice)
            throw new InvalidOperationException("Reset() only valid on owned-device instances (OplChip.Create). Reset externally on shared devices.");
        AdlMidiImports.adl_reset(_device);
        // adl_reset re-creates MIDIplay state. Re-apply our channel reservation.
        int rr = Native.adl_reserveChipChannels(_device, _chipId, AllChannelsMask);
        if (rr != 0)
            throw new InvalidOperationException($"adl_reserveChipChannels failed after reset: {rr}");
    }

    /// <summary>Release the underlying device when this instance owns it.</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_ownsDevice && _device != IntPtr.Zero)
            AdlMidiImports.adl_close(_device);
        GC.SuppressFinalize(this);
    }

    /// <summary>Finalizer — releases unmanaged memory if Dispose was not called.</summary>
    ~OplChip() { Dispose(); }

    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(OplChip));
    }

    private static class Native
    {
        private const string Lib = "libadlmidi";

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "adl_rt_rawOPL3")]
        public static extern int adl_rt_rawOPL3(IntPtr device, int chipId, ushort reg, byte value);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "adl_reserveChipChannels")]
        public static extern int adl_reserveChipChannels(IntPtr device, int chipId, uint channelMask);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "adl_getReservedChipChannels")]
        public static extern uint adl_getReservedChipChannels(IntPtr device, int chipId);
    }
}
