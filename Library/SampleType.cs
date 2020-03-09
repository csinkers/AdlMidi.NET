namespace ADLMidi.NET
{
    /// <summary>
    /// Sound output format
    /// </summary>
    public enum SampleType
    {
        S16 = 0, // signed PCM 16-bit
        S8,      // signed PCM 8-bit
        F32,     // float 32-bit
        F64,     // float 64-bit
        S24,     // signed PCM 24-bit
        S32,     // signed PCM 32-bit
        U8,      // unsigned PCM 8-bit
        U16,     // unsigned PCM 16-bit
        U24,     // unsigned PCM 24-bit
        U32,     // unsigned PCM 32-bit
        Count    // Count of available sample format types
    }
}
