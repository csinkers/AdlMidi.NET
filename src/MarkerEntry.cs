using System.Runtime.InteropServices;

namespace ADLMidi.NET;

/// <summary>
/// MIDI Marker structure
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct MarkerEntry
{
    public string Label;   // MIDI Marker title
    public double PosTime; // Absolute time position of the marker in seconds
    public uint PosTicks;  // Absolute time position of the marker in MIDI ticks
}