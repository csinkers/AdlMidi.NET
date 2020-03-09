using System.Runtime.InteropServices;

namespace ADLMidi.NET
{
    /// <summary>
    /// Identifier of dynamic bank
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BankId
    {
        public byte Percussive; // 0 if bank is melodic set, or 1 if bank is a percussion set
        public byte Msb; // Assign to MSB bank number
        public byte Lsb; // Assign to LSB bank number
    }
}
