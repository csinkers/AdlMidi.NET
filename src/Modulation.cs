using System;

namespace ADLMidi.NET
{
    [Flags]
    public enum Modulation : byte
    {
        // ReSharper disable InconsistentNaming
        // Two operator:
        FM = 0,
        AM = 1,
        // ReSharper restore InconsistentNaming

        Feedback1 = 2,
        Feedback2 = 4,
        Feedback3 = 6,
        Feedback4 = 8,
        Feedback5 = 10,
        Feedback6 = 12,
        Feedback7 = 14,
    }
}