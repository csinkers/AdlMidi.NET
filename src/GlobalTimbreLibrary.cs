using System.Collections.Generic;
using SerdesNet;

namespace ADLMidi.NET
{
    class TimbreHeader
    {
        public const int Size = 6;
        public byte MidiPatchNumber { get; set; }
        public byte MidiBankNumber { get; set; }
        public uint InstrumentDataOffset { get; set; }
        public bool IsSentinel => MidiPatchNumber == 0xff && MidiBankNumber == 0xff;

        public static TimbreHeader Serdes(TimbreHeader header, ISerializer s)
        {
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

    public class GlobalTimbreLibrary // Load and save .OPL / .AD files from AIL / Miles Sound System
    {
        public IList<TimbreData> Data { get; } = new List<TimbreData>();
        public static GlobalTimbreLibrary Serdes(GlobalTimbreLibrary library, ISerializer s)
        {
            library ??= new GlobalTimbreLibrary();
            var start = s.Offset;


            IList<TimbreHeader> headers = new List<TimbreHeader>();
            if (s.Mode == SerializerMode.Reading)
            {
                TimbreHeader header;
                do
                {
                    header = TimbreHeader.Serdes(null, s);
                    headers.Add(header);
                } while (!header.IsSentinel);
            }
            else
            {
                uint offset = (uint)(start + TimbreHeader.Size * library.Data.Count + 2);
                foreach (var data in library.Data)
                {
                    var header = new TimbreHeader
                    {
                        MidiPatchNumber = data.MidiPatchNumber,
                        MidiBankNumber = data.MidiBankNumber,
                        InstrumentDataOffset = offset
                    };

                    TimbreHeader.Serdes(header, s);
                    headers.Add(header);
                    offset += data.Length;
                }

                // Write sentinel
                TimbreHeader.Serdes(new TimbreHeader
                {
                    MidiPatchNumber = 0xff,
                    MidiBankNumber = 0xff,
                }, s);
            }

            s.List(nameof(library.Data), library.Data, headers.Count - 1, TimbreData.Serdes);
            for (int i = 0; i < headers.Count - 1; i++)
            {
                library.Data[i].MidiPatchNumber = headers[i].MidiPatchNumber;
                library.Data[i].MidiBankNumber = headers[i].MidiBankNumber;
            }

            return library;
        }
    }
}
