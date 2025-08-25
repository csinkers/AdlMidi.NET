using System;
using SerdesNet;

namespace ADLMidi.NET;

/// <summary>
/// Instrument data
/// </summary>
public class WoplInstrument
{
    /// <summary>
    /// The instrument name
    /// </summary>
    public string Name { get; set; }

    Instrument _data;

    static Instrument SerdesI(Instrument w, ISerdes s, int version)
    {
        if (s == null) throw new ArgumentNullException(nameof(s));
        w.NoteOffset1         = s.Int16  (nameof(Instrument.NoteOffset1),         w.NoteOffset1);
        w.NoteOffset2         = s.Int16  (nameof(Instrument.NoteOffset2),         w.NoteOffset2);
        w.MidiVelocityOffset  = s.Int8   (nameof(Instrument.MidiVelocityOffset),  w.MidiVelocityOffset);
        w.SecondVoiceDetune   = s.Int8   (nameof(Instrument.SecondVoiceDetune),   w.SecondVoiceDetune);
        w.PercussionKeyNumber = s.UInt8  (nameof(Instrument.PercussionKeyNumber), w.PercussionKeyNumber);
        w.Flags               = s.UInt8  (nameof(Instrument.Flags),               w.Flags);
        w.FbConn1C0           = s.EnumU8 (nameof(Instrument.FbConn1C0),          w.FbConn1C0);
        w.FbConn2C0           = s.EnumU8 (nameof(Instrument.FbConn2C0),          w.FbConn2C0);
        w.Operator0           = s.Object (nameof(Instrument.Operator0), w.Operator0, Operator.Serdes);
        w.Operator1           = s.Object (nameof(Instrument.Operator1), w.Operator1, Operator.Serdes);
        w.Operator2           = s.Object (nameof(Instrument.Operator2), w.Operator2, Operator.Serdes);
        w.Operator3           = s.Object (nameof(Instrument.Operator3), w.Operator3, Operator.Serdes);
        if (version >= 3)
        {
            w.DelayOnMs = s.UInt16(nameof(Instrument.DelayOnMs), w.DelayOnMs);
            w.DelayOffMs = s.UInt16(nameof(Instrument.DelayOffMs), w.DelayOffMs);
        }

        return w;
    }

    /// <summary>
    /// Returns a string representation of the current object.
    /// </summary>
    public override string ToString()
        => $"F:{FbConn1C0} {Operator0.Attack}:{Operator0.Decay} {Operator0.Sustain}:{Operator0.Release} {Operator0.Waveform} " +
           $"{Operator0.Flags} {Operator0.Level} {Operator1.Attack}:{Operator1.Decay} {Operator1.Sustain}:{Operator1.Release} " +
           $"{Operator1.Waveform} {Operator1.Flags} {Operator1.Level}";

    /// <summary>
    /// Serialize or deserialize the WoplInstrument
    /// </summary>
    public static WoplInstrument Serdes(SerdesName _, WoplInstrument w, ISerdes s, int version)
    {
        if (s == null) throw new ArgumentNullException(nameof(s));
        w ??= new WoplInstrument();
        w.Name = s.FixedLengthString(nameof(Name), w.Name, 32);
        w._data = s.Object(nameof(_data), w._data, (_, w2, s2) => SerdesI(w2, s2, version));
        return w;
    }

    /// <summary>
    /// The data version
    /// </summary>
    public int Version { get => _data.Version; set => _data.Version = value; }

    /// <summary>
    /// The first note offset
    /// </summary>
    public short NoteOffset1 { get => _data.NoteOffset1; set => _data.NoteOffset1 = value; }

    /// <summary>
    /// The second note offset
    /// </summary>
    public short NoteOffset2 { get => _data.NoteOffset2; set => _data.NoteOffset2 = value; }
    /// <summary>
    /// The MIDI velocity offset
    /// </summary>
    public sbyte MidiVelocityOffset { get => _data.MidiVelocityOffset; set => _data.MidiVelocityOffset = value; }

    /// <summary>
    /// The detuning factor of the second voice
    /// </summary>
    public sbyte SecondVoiceDetune { get => _data.SecondVoiceDetune; set => _data.SecondVoiceDetune = value; }

    /// <summary>
    /// The percussion key number
    /// </summary>
    public byte PercussionKeyNumber { get => _data.PercussionKeyNumber; set => _data.PercussionKeyNumber = value; }

    /// <summary>
    /// The instrument mode
    /// </summary>
    public InstrumentMode InstrumentMode
    {
        get => (InstrumentMode)(_data.Flags & 0x7);
        set => _data.Flags = (byte)((_data.Flags & ~0x7) | ((int)value & 0x7));
    }

    /// <summary>
    /// The rhythm mode
    /// </summary>
    public RhythmMode RhythmMode
    {
        get => (RhythmMode)((_data.Flags & 0x38) >> 3);
        set => _data.Flags = (byte)((_data.Flags & ~0x38) | (((int)value & 0x7) << 3));
    }

    /// <summary>
    /// The first feedback mode
    /// </summary>
    public Modulation FbConn1C0 { get => _data.FbConn1C0; set => _data.FbConn1C0 = value; }

    /// <summary>
    /// The second feedback mode
    /// </summary>
    public Modulation FbConn2C0 { get => _data.FbConn2C0; set => _data.FbConn2C0 = value; }

    /// <summary>
    /// The first operator
    /// </summary>
    public Operator Operator0 { get => _data.Operator0; set => _data.Operator0 = value; }

    /// <summary>
    /// The second operator
    /// </summary>
    public Operator Operator1 { get => _data.Operator1; set => _data.Operator1 = value; }

    /// <summary>
    /// The third operator
    /// </summary>
    public Operator Operator2 { get => _data.Operator2; set => _data.Operator2 = value; }

    /// <summary>
    /// The fourth operator
    /// </summary>
    public Operator Operator3 { get => _data.Operator3; set => _data.Operator3 = value; }

    /// <summary>
    /// The delay on time in milliseconds
    /// </summary>
    public ushort DelayOnMs { get => _data.DelayOnMs; set => _data.DelayOnMs = value; }

    /// <summary>
    /// The delay off time in milliseconds
    /// </summary>
    public ushort DelayOffMs { get => _data.DelayOffMs; set => _data.DelayOffMs = value; }
}