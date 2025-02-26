using System.Runtime.InteropServices;

namespace ADLMidi.NET;

/// <summary>
/// Instrument structure
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Instrument
{
    /// <summary>
    /// Version of the instrument object
    /// </summary>
    public int Version;

    /// <summary>
    /// MIDI note key (half-tone) offset for an instrument (or a first voice in pseudo-4-op mode)
    /// </summary>
    public short NoteOffset1;

    /// <summary>
    /// MIDI note key (half-tone) offset for a second voice in pseudo-4-op mode
    /// </summary>
    public short NoteOffset2;

    /// <summary>
    /// MIDI note velocity offset (taken from Apogee TMB format)
    /// </summary>
    public sbyte MidiVelocityOffset;

    /// <summary>
    /// Second voice detune level (taken from DMX OP2)
    /// </summary>
    public sbyte SecondVoiceDetune;

    /// <summary>
    /// Percussion MIDI base tone number at which this drum will be played
    /// </summary>
    public byte PercussionKeyNumber;

    /// <summary>
    /// Packed bitfield containing instrument and rhythm modes
    /// </summary>
    public byte Flags;

    /// <summary>
    /// Feedback &amp; Connection register for first and second operators
    /// </summary>
    public Modulation FbConn1C0;

    /// <summary>
    /// Feedback &amp; Connection register for third and fourth operators
    /// </summary>
    public Modulation FbConn2C0;

    /// <summary>
    /// Operators register data (0)
    /// </summary>
    public Operator Operator0;

    /// <summary>
    /// Operators register data (1)
    /// </summary>
    public Operator Operator1;

    /// <summary>
    /// Operators register data (2)
    /// </summary>
    public Operator Operator2;

    /// <summary>
    /// Operators register data (3)
    /// </summary>
    public Operator Operator3;

    /// <summary>
    /// Millisecond delay of sounding while key is on
    /// </summary>
    public ushort DelayOnMs;

    /// <summary>
    /// Millisecond delay of sounding after key off
    /// </summary>
    public ushort DelayOffMs;
}