namespace ADLMidi.NET;

/// <summary>
/// Sound output format
/// </summary>
public enum SampleType
{
    /// <summary>Signed PCM 16-bit</summary>
    S16 = 0,
    /// <summary>Signed PCM 8-bit</summary>
    S8,
    /// <summary>Float 32-bit</summary>
    F32,
    /// <summary>Float 64-bit</summary>
    F64,
    /// <summary>Signed PCM 24-bit</summary>
    S24,
    /// <summary>Signed PCM 32-bit</summary>
    S32,
    /// <summary>Unsigned PCM 8-bit</summary>
    U8,
    /// <summary>Unsigned PCM 16-bit</summary>
    U16,
    /// <summary>Unsigned PCM 24-bit</summary>
    U24,
    /// <summary>Unsigned PCM 32-bit</summary>
    U32,
    /// <summary>Count of available sample format types</summary>
    Count
}