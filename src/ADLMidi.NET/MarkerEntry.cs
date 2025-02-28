using System.Runtime.InteropServices;

namespace ADLMidi.NET;

/// <summary>
/// Represents a MIDI marker.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct MarkerEntry
{
    /// <summary>
    /// Gets or sets the title of the MIDI marker.
    /// </summary>
    public string Label;

    /// <summary>
    /// Gets or sets the absolute time position of the marker in seconds.
    /// </summary>
    public double PosTime;

    /// <summary>
    /// Gets or sets the absolute time position of the marker in MIDI ticks.
    /// </summary>
    public uint PosTicks;
}