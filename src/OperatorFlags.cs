using System;

namespace ADLMidi.NET
{
    [Flags]
    public enum OperatorFlags : byte
    {
        Freq1 = 1,
        Freq2 = 2,
        Freq3 = 3,
        Freq4 = 4,
        Freq5 = 5,
        Freq6 = 6,
        Freq7 = 7,
        Freq8 = 8,
        Freq9 = 9,
        Freq10 = 10,
        Freq11 = 11,
        Freq12 = 12,
        Freq13 = 13,
        Freq14 = 14,
        Freq15 = 15,

        EnvelopeScale = 0x10, // KSR
        Sustain = 0x20, // EG
        Vibrato = 0x40, // VIB
        Tremolo = 0x80, // AM
    }
}
