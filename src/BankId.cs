using System.Runtime.InteropServices;

namespace ADLMidi.NET;

/// <summary>
/// Identifier of dynamic bank
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct BankId
{
    /// <summary>
    /// 0 if bank is melodic set, or 1 if bank is a percussion set
    /// </summary>
    public byte Percussive;

    /// <summary>
    /// Assign to MSB bank number
    /// </summary>
    public byte Msb;

    /// <summary>
    /// Assign to LSB bank number
    /// </summary>
    public byte Lsb;
}