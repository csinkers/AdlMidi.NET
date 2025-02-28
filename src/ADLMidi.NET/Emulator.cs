namespace ADLMidi.NET;

/// <summary>
/// List of available OPL3 emulators
/// </summary>
public enum Emulator
{
    /// <summary>
    /// Nuked OPL3 v. 1.8
    /// </summary>
    Nuked = 0,

    /// <summary>
    /// Nuked OPL3 v. 1.7.4
    /// </summary>
    Nuked174,

    /// <summary>
    /// DosBox
    /// </summary>
    DosBox,

    /// <summary>
    /// Opal
    /// </summary>
    Opal,

    /// <summary>
    /// Java
    /// </summary>
    Java,

    /// <summary>
    /// Count instrument on the level
    /// </summary>
    End
}