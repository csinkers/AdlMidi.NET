using System;

namespace ADLMidi.NET;

/// <summary>
/// The tremolo/vibrato/sustain and freq multiplication flags for an OPL3 operator.
/// </summary>
[Flags]
public enum OperatorFlags : byte
{
    /// <summary>Frequency multiplier 1</summary>
    Freq1 = 1,
    /// <summary>Frequency multiplier 2</summary>
    Freq2 = 2,
    /// <summary>Frequency multiplier 3</summary>
    Freq3 = 3,
    /// <summary>Frequency multiplier 4</summary>
    Freq4 = 4,
    /// <summary>Frequency multiplier 5</summary>
    Freq5 = 5,
    /// <summary>Frequency multiplier 6</summary>
    Freq6 = 6,
    /// <summary>Frequency multiplier 7</summary>
    Freq7 = 7,
    /// <summary>Frequency multiplier 8</summary>
    Freq8 = 8,
    /// <summary>Frequency multiplier 9</summary>
    Freq9 = 9,
    /// <summary>Frequency multiplier 10</summary>
    Freq10 = 10,
    /// <summary>Frequency multiplier 11</summary>
    Freq11 = 11,
    /// <summary>Frequency multiplier 12</summary>
    Freq12 = 12,
    /// <summary>Frequency multiplier 13</summary>
    Freq13 = 13,
    /// <summary>Frequency multiplier 14</summary>
    Freq14 = 14,
    /// <summary>Frequency multiplier 15</summary>
    Freq15 = 15,

    /// <summary>
    /// Envelope scaling (KSR). When this is set, higher notes are shorter than lower notes.
    /// </summary>
    EnvelopeScale = 0x10, // KSR

    /// <summary>
    /// Sustain. When set, the operator's output level will be held at its sustain level until a key off event.
    /// </summary>
    Sustain = 0x20, // EG

    /// <summary>
    /// Frequency vibrato on/off.
    /// </summary>
    Vibrato = 0x40, // VIB

    /// <summary>
    /// Tremolo (amplitude vibrato) on/off.
    /// </summary>
    Tremolo = 0x80, // AM
}