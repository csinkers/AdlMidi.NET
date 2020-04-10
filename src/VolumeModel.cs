namespace ADLMidi.NET
{
    /// <summary>
    /// Volume scaling models
    /// </summary>
    public enum VolumeModel : byte
    {
        Auto = 0,       // Automatically chosen by the specific bank                                                // / </summary>
        Generic = 1,    // Linearized scaling model, most standard                                                  // / </summary>
        NativeOpl3 = 2, // Native OPL3's logarithmic volume scale                                                   // / </summary>
        Cmf = 2,        // Native OPL3's logarithmic volume scale. Alias.                                           // / </summary>
        Dmx = 3,        // Logarithmic volume scale, using volume map table. Used in DMX.                           // / </summary>
        Apogee = 4,     // Logarithmic volume scale, used in Apogee Sound System.                                   // / </summary>
        NineX = 5       // Approximated and shorted volume map table. Similar to general, but has less granularity. // / </summary>
    }
}
