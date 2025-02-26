namespace ADLMidi.NET;

/// <summary>
/// WOPL instrument bank
/// </summary>
public class WoplBank
{
    /// <summary>
    /// Maximum number of instruments in the bank
    /// </summary>
    public const int BankSize = 128;

    /// <summary>
    /// Maximum length of the bank name
    /// </summary>
    public const int MaxNameLength = 32;
    string _name;

    /// <summary>
    /// The bank ID
    /// </summary>
    public ushort Id { get; set; }

    /// <summary>
    /// The bank name
    /// </summary>
    public string Name
    {
        get => _name;
        set
        {
            _name = (value ?? "");
            if (_name.Length > MaxNameLength)
                _name = _name.Substring(0, MaxNameLength);
        }

    }

    /// <summary>
    /// The instruments in the bank
    /// </summary>
    public WoplInstrument[] Instruments { get; } = new WoplInstrument[BankSize];
}