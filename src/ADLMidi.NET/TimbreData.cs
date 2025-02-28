using System;
using SerdesNet;

namespace ADLMidi.NET;

/// <summary>
/// The tone/timbre data
/// </summary>
public class TimbreData
{
    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    public override string ToString()
        => $"F:{FeedbackConnection} {Carrier.Attack}:{Carrier.Decay} {Carrier.Sustain}:{Carrier.Release} " +
           $"{Carrier.Waveform} {Carrier.Flags} {Carrier.Level} {Modulation.Attack}:{Modulation.Decay} " +
           $"{Modulation.Sustain}:{Modulation.Release} {Modulation.Waveform} {Modulation.Flags} {Modulation.Level}";

    /// <summary>
    /// The MIDI patch number (from header)
    /// </summary>
    public byte MidiPatchNumber { get; set; }

    /// <summary>
    /// The MIDI bank number
    /// </summary>
    public byte MidiBankNumber { get; set; }

    /// <summary>
    /// The size of the struct, including the length field itself. 14 for OPL2 instruments, and 25(?) for OPL3.
    /// </summary>
    public ushort Length { get; set; }

    /// <summary>
    /// Precise meaning unknown
    /// </summary>
    public byte Transpose { get; set; }

    /// <summary>
    /// Feedback/connection
    /// </summary>
    public Modulation FeedbackConnection { get; set; }

    /// <summary>
    /// The modulation type.
    /// </summary>
    public Operator Modulation { get; set; }

    /// <summary>
    /// The carrier wave type.
    /// </summary>
    public Operator Carrier { get; set; }

    /// <summary>
    /// Serialize or deserialize the TimbreData struct
    /// </summary>
    public static TimbreData Serdes(int _, TimbreData data, ISerdes s)
    {
        if (s == null) throw new ArgumentNullException(nameof(s));
        data ??= new TimbreData();
        data.Length             = s.UInt16(nameof(Length), data.Length);
        data.Transpose          = s.UInt8(nameof(Transpose), data.Transpose);
        data.Modulation         = s.Object(nameof(data.Modulation), data.Modulation, Operator.Serdes);
        data.FeedbackConnection = s.EnumU8(nameof(FeedbackConnection), data.FeedbackConnection);
        data.Carrier            = s.Object(nameof(data.Carrier), data.Carrier, Operator.Serdes);
        return data;
    }
}