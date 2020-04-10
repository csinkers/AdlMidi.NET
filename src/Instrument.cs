using System.Runtime.InteropServices;

namespace ADLMidi.NET
{
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
        /// <summary>
        /// @var inst_flags Instrument flags
        ///
        /// Enums: #ADL_InstrumentFlags and #ADL_RhythmMode
        ///
        /// Bitwise flags bit map:
        /// [0EEEDCBA]
        ///  A) 0x00 - 2-operator mode
        ///  B) 0x01 - 4-operator mode
        ///  C) 0x02 - pseudo-4-operator (two 2-operator voices) mode
        ///  D) 0x04 - is 'blank' instrument (instrument which has no sound)
        ///  E) 0x38 - Reserved for rhythm-mode percussion type number (three bits number)
        ///     -&gt; 0x00 - Melodic or Generic drum (rhythm-mode is disabled)
        ///     -&gt; 0x08 - is Bass drum
        ///     -&gt; 0x10 - is Snare
        ///     -&gt; 0x18 - is Tom-tom
        ///     -&gt; 0x20 - is Cymbal
        ///     -&gt; 0x28 - is Hi-hat
        ///  0) Reserved / Unused
        /// </summary>
        public InstrumentFlags Flags;
        public Modulation FbConn1C0;     // Feedback & Connection register for first and second operators
        public Modulation FbConn2C0;     // Feedback & Connection register for third and fourth operators
        public Operator Operator0; // Operators register data
        public Operator Operator1;
        public Operator Operator2;
        public Operator Operator3;
        public ushort DelayOnMs;   // Millisecond delay of sounding while key is on
        public ushort DelayOffMs;  // Millisecond delay of sounding after key off
    }
}
