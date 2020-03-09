using SerdesNet;

namespace ADLMidi.NET
{
    public class TimbreData
    {
        public override string ToString()
            => $"F:{FeedbackConnection} {Carrier.Attack}:{Carrier.Decay} {Carrier.Sustain}:{Carrier.Release} {Carrier.Waveform} {Carrier.Flags} {Carrier.Level} {Modulation.Attack}:{Modulation.Decay} {Modulation.Sustain}:{Modulation.Release} {Modulation.Waveform} {Modulation.Flags} {Modulation.Level}";

        public byte MidiPatchNumber { get; set; } // From header
        public byte MidiBankNumber { get; set; }

        public ushort Length { get; set; }  // Struct length, including the length field itself. Value is always 14 for OPL2 instruments, and 25(?) for OPL3.
        public byte Transpose { get; set; }  // ! Precise meaning unknown
        public Modulation FeedbackConnection { get; set; }  //  Feedback/connection

        public Operator Modulation { get; set; }
        public Operator Carrier { get; set; }

        public static TimbreData Serdes(int i, TimbreData data, ISerializer s)
        {
            data ??= new TimbreData();
            data.Length             = s.UInt16(nameof(Length), data.Length);
            data.Transpose          = s.UInt8(nameof(Transpose), data.Transpose);
            data.Modulation         = s.Meta(nameof(data.Modulation), data.Modulation, Operator.Serdes);
            data.FeedbackConnection = s.EnumU8(nameof(FeedbackConnection), data.FeedbackConnection);
            data.Carrier            = s.Meta(nameof(data.Carrier), data.Carrier, Operator.Serdes);
            return data;
        }
    }
}