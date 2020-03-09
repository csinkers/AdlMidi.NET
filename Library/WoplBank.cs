namespace ADLMidi.NET
{
    public class WoplBank
    {
        public const int BankSize = 128;
        public const int MaxNameLength = 32;
        string _name;

        public ushort Id { get; set; }
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
        public WoplInstrument[] Instruments { get; } = new WoplInstrument[BankSize];
    }
}