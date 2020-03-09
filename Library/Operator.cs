using System.Runtime.InteropServices;
using SerdesNet;

namespace ADLMidi.NET
{
    /// <summary>
    /// Operator structure, part of Instrument structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Operator
    {
        byte Z_KeyScaleLevel;  // Key Scale Level / Total level register data
        byte Z_AttackDecay;    // Attack / Decay
        byte Z_SustainRelease; // Sustain and Release register data

        public OperatorFlags Flags; // AM/Vib/Env/Ksr/FMult characteristics
        public Waveform Waveform;   // Wave form

        public int Attack => (Z_AttackDecay & 0xf0) >> 4; // 0..15
        public int Decay => Z_AttackDecay & 0x0f; // 0..15
        public int Sustain => 0xf - ((Z_SustainRelease & 0xf0) >> 4); // 0..15
        public int Release => Z_SustainRelease & 0x0f; // 0..15
        public int Level => (63 - Z_KeyScaleLevel) & 0x3f; // 0..63
        public int KeyScale => (Z_KeyScaleLevel & 0xc0) >> 6; // 0..3
        public int FreqMultiple => (int)Flags & 0xf; // 0..15
        public static Operator Blank => new Operator
        {
            Z_KeyScaleLevel = 63,
            Z_SustainRelease = 240
        };

        public static Operator Serdes(int i, Operator o, ISerializer s)
        {
            o.Flags = s.EnumU8(nameof(Flags), o.Flags);
            o.Z_KeyScaleLevel = s.UInt8(nameof(Z_KeyScaleLevel), o.Z_KeyScaleLevel);
            o.Z_AttackDecay = s.UInt8(nameof(Z_AttackDecay), o.Z_AttackDecay);
            o.Z_SustainRelease = s.UInt8(nameof(Z_SustainRelease), o.Z_SustainRelease);
            o.Waveform = s.EnumU8(nameof(Waveform), o.Waveform);
            return o;
        }
    }
}
