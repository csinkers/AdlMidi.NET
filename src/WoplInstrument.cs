using SerdesNet;

namespace ADLMidi.NET
{
    public class WoplInstrument
    {
        public string Name { get; set; }
        Instrument _data;

        static Instrument SerdesI(int i, Instrument w, ISerializer s)
        {
            w.NoteOffset1         = s.Int16  (nameof(Instrument.NoteOffset1),         w.NoteOffset1);
            w.NoteOffset2         = s.Int16  (nameof(Instrument.NoteOffset2),         w.NoteOffset2);
            w.MidiVelocityOffset  = s.Int8   (nameof(Instrument.MidiVelocityOffset),  w.MidiVelocityOffset);
            w.SecondVoiceDetune   = s.Int8   (nameof(Instrument.SecondVoiceDetune),   w.SecondVoiceDetune);
            w.PercussionKeyNumber = s.UInt8  (nameof(Instrument.PercussionKeyNumber), w.PercussionKeyNumber);
            w.Flags               = s.UInt8  (nameof(Instrument.Flags),               w.Flags);
            w.FbConn1C0           = s.EnumU8 (nameof(Instrument.FbConn1C0),          w.FbConn1C0);
            w.FbConn2C0           = s.EnumU8 (nameof(Instrument.FbConn2C0),          w.FbConn2C0);
            w.Operator0           = s.Meta  (nameof(Instrument.Operator0), w.Operator0, Operator.Serdes);
            w.Operator1           = s.Meta  (nameof(Instrument.Operator1), w.Operator1, Operator.Serdes);
            w.Operator2           = s.Meta  (nameof(Instrument.Operator2), w.Operator2, Operator.Serdes);
            w.Operator3           = s.Meta  (nameof(Instrument.Operator3), w.Operator3, Operator.Serdes);
            if (s.PeekVersion() >= 3)
            {
                w.DelayOnMs = s.UInt16(nameof(Instrument.DelayOnMs), w.DelayOnMs);
                w.DelayOffMs = s.UInt16(nameof(Instrument.DelayOffMs), w.DelayOffMs);
            }

            return w;
        }

        public override string ToString()
            => $"F:{FbConn1C0} {Operator0.Attack}:{Operator0.Decay} {Operator0.Sustain}:{Operator0.Release} {Operator0.Waveform} {Operator0.Flags} {Operator0.Level} {Operator1.Attack}:{Operator1.Decay} {Operator1.Sustain}:{Operator1.Release} {Operator1.Waveform} {Operator1.Flags} {Operator1.Level}";
        public static WoplInstrument Serdes(int i, WoplInstrument w, ISerializer s)
        {
            w ??= new WoplInstrument();
            w.Name = s.FixedLengthString(nameof(Name), w.Name, 32);
            w._data = s.Meta(nameof(_data), w._data, SerdesI);
            return w;
        }

        public int Version { get => _data.Version; set => _data.Version = value; }
        public short NoteOffset1 { get => _data.NoteOffset1; set => _data.NoteOffset1 = value; }
        public short NoteOffset2 { get => _data.NoteOffset2; set => _data.NoteOffset2 = value; }
        public sbyte MidiVelocityOffset { get => _data.MidiVelocityOffset; set => _data.MidiVelocityOffset = value; }
        public sbyte SecondVoiceDetune { get => _data.SecondVoiceDetune; set => _data.SecondVoiceDetune = value; }
        public byte PercussionKeyNumber { get => _data.PercussionKeyNumber; set => _data.PercussionKeyNumber = value; }

        public InstrumentMode InstrumentMode
        {
            get => (InstrumentMode)(_data.Flags & 0x7);
            set => _data.Flags = (byte)((_data.Flags & ~0x7) | ((int)value & 0x7));
        }

        public RhythmMode RhythmMode
        {
            get => (RhythmMode)((_data.Flags & 0x38) >> 3);
            set => _data.Flags = (byte)((_data.Flags & ~0x38) | (((int)value & 0x7) << 3));
        }

        public Modulation FbConn1C0 { get => _data.FbConn1C0; set => _data.FbConn1C0 = value; }
        public Modulation FbConn2C0 { get => _data.FbConn2C0; set => _data.FbConn2C0 = value; }
        public Operator Operator0 { get => _data.Operator0; set => _data.Operator0 = value; }
        public Operator Operator1 { get => _data.Operator1; set => _data.Operator1 = value; }
        public Operator Operator2 { get => _data.Operator2; set => _data.Operator2 = value; }
        public Operator Operator3 { get => _data.Operator3; set => _data.Operator3 = value; }
        public ushort DelayOnMs { get => _data.DelayOnMs; set => _data.DelayOnMs = value; }
        public ushort DelayOffMs { get => _data.DelayOffMs; set => _data.DelayOffMs = value; }
    }
}
