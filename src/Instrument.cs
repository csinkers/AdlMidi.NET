using System.Runtime.InteropServices;

namespace ADLMidi.NET;

/// <summary>
/// Instrument structure
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Instrument
{
    public int Version;              // Version of the instrument object
    public short NoteOffset1;        // MIDI note key (half-tone) offset for an instrument (or a first voice in pseudo-4-op mode)
    public short NoteOffset2;        // MIDI note key (half-tone) offset for a second voice in pseudo-4-op mode
    public sbyte MidiVelocityOffset; // MIDI note velocity offset (taken from Apogee TMB format)
    public sbyte SecondVoiceDetune;  // Second voice detune level (taken from DMX OP2)
    public byte PercussionKeyNumber; // Percussion MIDI base tone number at which this drum will be played
    public byte Flags; // Packed bitfield containing instrument and rhythm modes
    public Modulation FbConn1C0;     // Feedback & Connection register for first and second operators
    public Modulation FbConn2C0;     // Feedback & Connection register for third and fourth operators
    public Operator Operator0; // Operators register data
    public Operator Operator1;
    public Operator Operator2;
    public Operator Operator3;
    public ushort DelayOnMs;   // Millisecond delay of sounding while key is on
    public ushort DelayOffMs;  // Millisecond delay of sounding after key off
}