namespace ADLMidi.NET;

/// <summary>
/// Flags for dynamic bank access
/// </summary>
public enum BankAccessFlags
{
    /// <summary>
    /// Create bank, allocating memory as needed
    /// </summary>
    Create = 1,

    /// <summary>
    /// Create bank, never allocating memory
    /// </summary>
    CreateRt = 1 | 2
}