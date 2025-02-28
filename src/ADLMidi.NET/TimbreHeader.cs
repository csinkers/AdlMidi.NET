using System;
using SerdesNet;

namespace ADLMidi.NET;

internal class TimbreHeader
{
    public const int Size = 6;
    public byte MidiPatchNumber { get; set; }
    public byte MidiBankNumber { get; set; }
    public uint InstrumentDataOffset { get; set; }
    public bool IsSentinel => MidiPatchNumber == 0xff && MidiBankNumber == 0xff;

    public static TimbreHeader Serdes(TimbreHeader header, ISerdes s)
    {
        if (s == null) throw new ArgumentNullException(nameof(s));
        header ??= new TimbreHeader();
        header.MidiPatchNumber = s.UInt8(nameof(MidiPatchNumber), header.MidiPatchNumber);
        header.MidiBankNumber = s.UInt8(nameof(MidiBankNumber), header.MidiBankNumber);

        if (header.IsSentinel)
        {
            header.InstrumentDataOffset = 0;
            return header;
        }

        header.InstrumentDataOffset = s.UInt32(nameof(InstrumentDataOffset), header.InstrumentDataOffset);
        return header;
    }
}