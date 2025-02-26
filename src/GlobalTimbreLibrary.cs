using System;
using System.Collections.Generic;
using SerdesNet;

namespace ADLMidi.NET;

/// <summary>
/// Load and save .OPL / .AD files from AIL / Miles Sound System
/// </summary>
public class GlobalTimbreLibrary
{
    /// <summary>
    /// The timbre data
    /// </summary>
    public IList<TimbreData> Data { get; } = new List<TimbreData>();

    /// <summary>
    /// Serialize or deserialize the global timbre library
    /// </summary>
    /// <param name="library">The library to serialize from / deserialize into. If this is null when deserializing, then a new instance will be created and returned.</param>
    /// <param name="s">The serializer/deserializer</param>
    /// <returns>The library that was (de)serialized.</returns>
    public static GlobalTimbreLibrary Serdes(GlobalTimbreLibrary library, ISerdes s)
    {
        if (s == null) throw new ArgumentNullException(nameof(s));
        library ??= new GlobalTimbreLibrary();
        var start = s.Offset;


        IList<TimbreHeader> headers = new List<TimbreHeader>();
        if (s.IsReading())
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