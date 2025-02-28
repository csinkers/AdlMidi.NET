namespace ADLMidi.NET;

/// <summary>
/// Volume scaling models
/// </summary>
public enum VolumeModel : byte
{
    /// <summary>Automatically chosen by the specific bank</summary>
    Auto = 0,
    /// <summary>Linearized scaling model, most standard</summary>
    Generic = 1,
    /// <summary>Native OPL3's logarithmic volume scale</summary>
    NativeOpl3 = 2,
    /// <summary>Native OPL3's logarithmic volume scale. Alias.</summary>
    Cmf = 2,
    /// <summary>Logarithmic volume scale, using volume map table. Used in DMX.</summary>
    Dmx = 3,
    /// <summary>Logarithmic volume scale, used in Apogee Sound System.</summary>
    Apogee = 4,
    /// <summary>Approximated and shorted volume map table. Similar to general, but has less granularity.</summary>
    NineX = 5
}