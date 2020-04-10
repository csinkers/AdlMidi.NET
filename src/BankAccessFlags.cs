namespace ADLMidi.NET
{
    /// <summary>
    /// Flags for dynamic bank access
    /// </summary>
    public enum BankAccessFlags
    {
        Create = 1, // create bank, allocating memory as needed
        CreateRt = 1 | 2 // create bank, never allocating memory
    }
}
