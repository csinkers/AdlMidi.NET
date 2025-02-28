using System;

namespace ADLMidi.NET;

/// <summary>
/// Flags for modulation settings in the ADLMIDI library.
/// </summary>
[Flags]
public enum Modulation : byte
{
    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Frequency Modulation (FM) mode.
    /// </summary>
    FM = 0,

    /// <summary>
    /// Amplitude Modulation (AM) mode.
    /// </summary>
    AM = 1,

    // ReSharper restore InconsistentNaming

    /// <summary>
    /// Feedback level 1.
    /// </summary>
    Feedback1 = 2,

    /// <summary>
    /// Feedback level 2.
    /// </summary>
    Feedback2 = 4,

    /// <summary>
    /// Feedback level 3.
    /// </summary>
    Feedback3 = 6,

    /// <summary>
    /// Feedback level 4.
    /// </summary>
    Feedback4 = 8,

    /// <summary>
    /// Feedback level 5.
    /// </summary>
    Feedback5 = 10,

    /// <summary>
    /// Feedback level 6.
    /// </summary>
    Feedback6 = 12,

    /// <summary>
    /// Feedback level 7.
    /// </summary>
    Feedback7 = 14,
}