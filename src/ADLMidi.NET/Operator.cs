using System;
using System.Runtime.InteropServices;
using SerdesNet;

namespace ADLMidi.NET;

/// <summary>
/// Operator structure, part of Instrument structure
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Operator
{
    /// <summary>
    /// Key Scale Level / Total level register data
    /// </summary>
    byte Z_KeyScaleLevel;

    /// <summary>
    /// Attack / Decay
    /// </summary>
    byte Z_AttackDecay;

    /// <summary>
    /// Sustain and Release register data
    /// </summary>
    byte Z_SustainRelease;
    /// <summary>
    /// AM/Vib/Env/Ksr/FMult characteristics
    /// </summary>
    public OperatorFlags Flags;

    /// <summary>
    /// Wave form
    /// </summary>
    public Waveform Waveform;

    /// <summary>
    /// Attack parameter of the operator
    /// </summary>
    public int Attack => (Z_AttackDecay & 0xf0) >> 4; // 0..15

    /// <summary>
    /// Decay parameter of the operator
    /// </summary>
    public int Decay => Z_AttackDecay & 0x0f; // 0..15

    /// <summary>
    /// Sustain parameter of the operator
    /// </summary>
    public int Sustain => 0xf - ((Z_SustainRelease & 0xf0) >> 4); // 0..15

    /// <summary>
    /// Release parameter of the operator
    /// </summary>
    public int Release => Z_SustainRelease & 0x0f; // 0..15

    /// <summary>
    /// Level of the operator
    /// </summary>
    public int Level => (63 - Z_KeyScaleLevel) & 0x3f; // 0..63

    /// <summary>
    /// Key scale level of the operator
    /// </summary>
    public int KeyScale => (Z_KeyScaleLevel & 0xc0) >> 6; // 0..3

    /// <summary>
    /// Frequency multiplier of the operator
    /// </summary>
    public int FreqMultiple => (int)Flags & 0xf; // 0..15

    /// <summary>
    /// Returns a blank operator
    /// </summary>
    public static Operator Blank => new()
    {
        Z_KeyScaleLevel = 63,
        Z_SustainRelease = 240
    };

    /// <summary>
    /// Serializes or deserializes an Operator object
    /// </summary>
    public static Operator Serdes(string _, Operator o, ISerdes s)
    {
        if (s == null) throw new ArgumentNullException(nameof(s));
        o.Flags = s.EnumU8(nameof(Flags), o.Flags);
        o.Z_KeyScaleLevel = s.UInt8(nameof(Z_KeyScaleLevel), o.Z_KeyScaleLevel);
        o.Z_AttackDecay = s.UInt8(nameof(Z_AttackDecay), o.Z_AttackDecay);
        o.Z_SustainRelease = s.UInt8(nameof(Z_SustainRelease), o.Z_SustainRelease);
        o.Waveform = s.EnumU8(nameof(Waveform), o.Waveform);
        return o;
    }
}