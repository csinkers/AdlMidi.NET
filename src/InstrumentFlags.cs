namespace ADLMidi.NET;

/// <summary>
/// Modes for instruments in the ADLMIDI library.
/// </summary>
public enum InstrumentMode
{
    /// <summary>
    /// Two-operator mode.
    /// </summary>
    TwoOperator = 0,

    /// <summary>
    /// Four-operator mode.
    /// </summary>
    FourOperator = 1,

    /// <summary>
    /// Pseudo four-operator mode.
    /// </summary>
    PseudoFourOperator = 2,

    /// <summary>
    /// Blank mode.
    /// </summary>
    Blank = 4
}