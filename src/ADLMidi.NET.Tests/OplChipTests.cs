using System;
using ADLMidi.NET;
using Xunit;

namespace ADLMidi.NET.Tests;

public class OplChipTests : IClassFixture<DllImportFixture>
{
    public OplChipTests(DllImportFixture _) { }

    [Fact]
    public void Silent_chip_generates_zero_samples()
    {
        using var chip = OplChip.Create(44100);
        var buf = new short[2 * 1024];
        chip.GenerateFrames(buf, 1024);
        foreach (var s in buf) Assert.Equal(0, s);
    }

    // Drives a minimal OPL2 note-on on channel 0 and asserts the chip produces
    // audible output. This is not a tone-accuracy check — just a smoke test that
    // our Write/Generate sequence reaches nuked-opl3's audio path.
    [Fact]
    public void Note_on_channel_0_generates_nonzero_samples()
    {
        using var chip = OplChip.Create(44100);

        chip.WriteReg(0x01, 0x20);  // enable waveform select (OPL2)
        chip.WriteReg(0x20, 0x01);  // modulator: MULT=1
        chip.WriteReg(0x23, 0x01);  // carrier:   MULT=1
        chip.WriteReg(0x40, 0x10);  // modulator: TL (attenuate)
        chip.WriteReg(0x43, 0x00);  // carrier:   TL=0
        chip.WriteReg(0x60, 0xF0);  // modulator: AR=15 DR=0
        chip.WriteReg(0x63, 0xF0);  // carrier:   AR=15 DR=0
        chip.WriteReg(0x80, 0x00);  // modulator: SL=0 RR=0
        chip.WriteReg(0x83, 0x00);  // carrier:   SL=0 RR=0
        chip.WriteReg(0xA0, 0x98);  // FNum low
        chip.WriteReg(0xB0, 0x31);  // KeyOn=1, Block=2, FNum-high=1
        chip.WriteReg(0xC0, 0x01);  // FB=0, CON=1 (additive)

        var buf = new short[2 * 4096];
        chip.GenerateFrames(buf, 4096);

        int nonZero = 0;
        foreach (var s in buf) if (s != 0) nonZero++;
        Assert.True(nonZero > 100, $"expected audible output, got {nonZero} non-zero samples");
    }

    [Fact]
    public void Reset_silences_chip()
    {
        using var chip = OplChip.Create(44100);
        chip.WriteReg(0xB0, 0x31);      // key on
        chip.Reset();
        var buf = new short[2 * 1024];
        chip.GenerateFrames(buf, 1024);
        foreach (var s in buf) Assert.Equal(0, s);
    }
}
