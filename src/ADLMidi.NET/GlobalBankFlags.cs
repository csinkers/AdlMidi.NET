using System;

namespace ADLMidi.NET;

/// <summary>
/// Flags for global bank settings in the ADLMIDI library.
/// </summary>
[Flags]
public enum GlobalBankFlags : byte
{
    /// <summary>
    /// Enables deep tremolo effect.
    /// </summary>
    DeepTremolo = 1,

    /// <summary>
    /// Enables deep vibrato effect.
    /// </summary>
    DeepVibrato = 2,
}