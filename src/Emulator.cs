namespace ADLMidi.NET
{
    /// <summary>
    /// List of available OPL3 emulators
    /// </summary>
    public enum Emulator
    {
        Nuked = 0, // Nuked OPL3 v. 1.8
        Nuked174,  // Nuked OPL3 v. 1.7.4
        DosBox,    // DosBox
        Opal,      // Opal
        Java,      // Java
        End        // Count instrument on the level
    }
}
