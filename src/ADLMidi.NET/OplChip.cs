using System;
using System.Runtime.InteropServices;

namespace ADLMidi.NET;

/// <summary>
/// Bare OPL3 chip wrapper around nuked-opl3 as shipped inside libadlmidi.
/// Lets callers drive raw register writes and pull stereo PCM frames, bypassing
/// libadlmidi's MIDI / bank / sequencer layers. Intended for use cases that
/// synthesize their own register streams (e.g. Ultima Underworld TVFX effects).
///
/// Backed by libadlmidi's <c>adl_barechip_*</c> ABI (opaque-handle allocator +
/// write / generate / reset). These symbols are exported with
/// <c>ADLMIDI_DECLSPEC</c> so they are visible in the shared library across
/// all platforms; the earlier revision of this class P/Invoked the raw
/// <c>OPL3_*</c> helpers which are internal to libadlmidi and not exported
/// from the Windows DLL or Linux .so.
/// </summary>
public sealed class OplChip : IDisposable
{
    private IntPtr _chip;
    private bool _disposed;
    private readonly int _sampleRateHz;

    private OplChip(IntPtr chip, int sampleRateHz)
    {
        _chip = chip;
        _sampleRateHz = sampleRateHz;
    }

    /// <summary>
    /// Create a chip initialised for the given sample rate. The chip behaves as
    /// an OPL3 (which is register-compatible with OPL2 in the low register
    /// space, so OPL2-only callers ignore OPL3-specific registers).
    /// </summary>
    public static OplChip Create(int sampleRateHz)
    {
        if (sampleRateHz <= 0) throw new ArgumentOutOfRangeException(nameof(sampleRateHz));

        IntPtr handle = Native.adl_barechip_new(sampleRateHz);
        if (handle == IntPtr.Zero)
            throw new InvalidOperationException("adl_barechip_new returned null");
        return new OplChip(handle, sampleRateHz);
    }

    /// <summary>Write <paramref name="val"/> to OPL register <paramref name="addr"/>.</summary>
    public void WriteReg(int addr, byte val)
    {
        ThrowIfDisposed();
        Native.adl_barechip_write_reg(_chip, (uint)addr, val);
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
        Native.adl_barechip_generate(_chip, frames, interleavedStereo);
    }

    /// <summary>Reset the chip to its power-on state, preserving sample rate.</summary>
    public void Reset()
    {
        ThrowIfDisposed();
        Native.adl_barechip_reset(_chip, _sampleRateHz);
    }

    /// <summary>Release the chip's unmanaged memory.</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_chip != IntPtr.Zero)
        {
            Native.adl_barechip_free(_chip);
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

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "adl_barechip_new")]
        public static extern IntPtr adl_barechip_new(int sampleRateHz);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "adl_barechip_free")]
        public static extern void adl_barechip_free(IntPtr chip);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "adl_barechip_write_reg")]
        public static extern void adl_barechip_write_reg(IntPtr chip, uint addr, byte val);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "adl_barechip_generate")]
        public static extern void adl_barechip_generate(IntPtr chip, int frames,
            [In, Out] short[] outBuf);

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "adl_barechip_reset")]
        public static extern void adl_barechip_reset(IntPtr chip, int sampleRateHz);
    }
}
