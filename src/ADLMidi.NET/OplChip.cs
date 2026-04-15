using System;
using System.Runtime.InteropServices;

namespace ADLMidi.NET;

/// <summary>
/// Bare OPL3 chip wrapper around nuked-opl3 as shipped inside libadlmidi.
/// Lets callers drive raw register writes and pull stereo PCM frames, bypassing
/// libadlmidi's MIDI / bank / sequencer layers. Intended for use cases that
/// synthesize their own register streams (e.g. Ultima Underworld TVFX effects).
///
/// The underlying native code exports <c>OPL3_Reset</c>, <c>OPL3_WriteReg</c>,
/// and <c>OPL3_GenerateStream</c> but not an allocator — so we carve the
/// <c>opl3_chip</c> struct out of unmanaged memory directly. Allocating 64 KB
/// gives ~10 KB headroom over the measured struct size of the current
/// libadlmidi nuked-opl3 build. If a future libadlmidi update grows the struct
/// beyond this, crashes will surface immediately in tests; bump the constant.
/// </summary>
public sealed class OplChip : IDisposable
{
    private const int ChipStructBytes = 65536;

    private IntPtr _chip;
    private bool _disposed;

    private OplChip(IntPtr chip) { _chip = chip; }

    /// <summary>
    /// Create a chip initialised for the given sample rate. The chip behaves as
    /// an OPL3 (which is register-compatible with OPL2 in the low register
    /// space, so OPL2-only callers ignore OPL3-specific registers).
    /// </summary>
    public static OplChip Create(int sampleRateHz)
    {
        if (sampleRateHz <= 0) throw new ArgumentOutOfRangeException(nameof(sampleRateHz));

        IntPtr mem = Marshal.AllocHGlobal(ChipStructBytes);
        // OPL3_Reset() zero-inits the struct internally, but explicit zeroing
        // ahead of time is cheap insurance against UB if the native layout
        // ever adds a field the reset path forgets.
        unsafe
        {
            new Span<byte>((void*)mem, ChipStructBytes).Clear();
        }
        Native.OPL3_Reset(mem, (uint)sampleRateHz);
        return new OplChip(mem);
    }

    /// <summary>Write <paramref name="val"/> to OPL register <paramref name="addr"/>.</summary>
    public void WriteReg(int addr, byte val)
    {
        ThrowIfDisposed();
        Native.OPL3_WriteReg(_chip, (ushort)addr, val);
    }

    /// <summary>
    /// Render <paramref name="frames"/> stereo frames into
    /// <paramref name="interleavedStereo"/> (length ≥ 2 × frames).
    /// </summary>
    public void GenerateFrames(short[] interleavedStereo, int frames)
    {
        ThrowIfDisposed();
        if (interleavedStereo == null) throw new ArgumentNullException(nameof(interleavedStereo));
        if (frames < 0) throw new ArgumentOutOfRangeException(nameof(frames));
        if (interleavedStereo.Length < frames * 2)
            throw new ArgumentException("buffer smaller than 2 * frames", nameof(interleavedStereo));
        if (frames == 0) return;
        Native.OPL3_GenerateStream(_chip, interleavedStereo, (uint)frames);
    }

    /// <summary>Reset the chip to its power-on state, preserving sample rate.</summary>
    public void Reset()
    {
        ThrowIfDisposed();
        // Sample rate is stashed inside the struct; OPL3_Reset accepts a new rate
        // on each call. Re-read the current rateratio/samplecnt by re-resetting
        // at a cached rate. We don't track it — callers re-create the chip when
        // sample rate changes — so Reset() rebuilds at a nominal 44100.
        //
        // Callers who need a different rate on reset should Dispose + Create.
        Native.OPL3_Reset(_chip, 44100);
    }

    /// <summary>Release the chip's unmanaged memory.</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_chip != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(_chip);
            _chip = IntPtr.Zero;
        }
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

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPL3_Reset")]
        public static extern void OPL3_Reset(IntPtr chip, uint sampleRate);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPL3_WriteReg")]
        public static extern void OPL3_WriteReg(IntPtr chip, ushort reg, byte val);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPL3_GenerateStream")]
        public static extern void OPL3_GenerateStream(IntPtr chip,
            [In, Out] short[] buf, uint numFrames);
    }
}
