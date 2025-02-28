namespace ADLMidi.NET;

/// <summary>
/// Waveform type
/// </summary>
public enum Waveform : byte
{
    /// <summary>
    /// Sine wave
    /// </summary>
    Sine = 0,
    /// <summary>
    /// Half sine wave
    /// </summary>
    HalfSine,
    /// <summary>
    /// Absolute sine wave
    /// </summary>
    AbsSine,
    /// <summary>
    /// Pulse-sine
    /// </summary>
    PulseSine,
    /// <summary>
    /// Sine (even periods only)
    /// </summary>
    SineEvenPeriods,
    /// <summary>
    /// Absolute sine (even periods only)
    /// </summary>
    AbsSineEventPeriods,
    /// <summary>
    /// Square wave
    /// </summary>
    Square,
    /// <summary>
    /// Derived square wave
    /// </summary>
    DerivedSquare
}