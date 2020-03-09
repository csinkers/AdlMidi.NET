namespace ADLMidi.NET
{
    /// <summary>
    /// Instrument flags
    /// </summary>
    public enum InstrumentFlags : byte
    {
        TwoOp = 0x00,          // Is two-operator single-voice instrument (no flags)
        FourOp = 0x01,         // Is true four-operator instrument
        Pseudo4Op = 0x02,      // Is pseudo four-operator (two 2-operator voices) instrument
        IsBlank = 0x04,        // Is a blank instrument entry
        RhythmModeMask = 0x38, // RhythmMode flags mask
        AllMask = 0x07         // Mask of the flags range
    }
}
