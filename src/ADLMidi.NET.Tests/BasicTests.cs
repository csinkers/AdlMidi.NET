namespace ADLMidi.NET.Tests;

public class BasicTests : IClassFixture<DllImportFixture>
{
    public BasicTests(DllImportFixture _)
    {
    }

    [Fact]
    public void CreateDisposeTest()
    {
        using var player = AdlMidi.Init();
    }

    [Fact]
    public void PlayXmiTest()
    {
        const int expectedLength = 16983;
        byte[] expectedBytes = new byte[expectedLength];
        new byte[] { 0x57, 0x4f, 0x50, 0x4c, 0x33, 0x2d, 0x42, 0x41, 0x4e, 0x4b, 0x00, 0x03, 0x00, 0x00, 0x01, 0x00, 0x01, 0x03, }.CopyTo(expectedBytes, 0);

        using var player = AdlMidi.Init();

        GlobalTimbreLibrary timbreLibrary = new GlobalTimbreLibrary();
        var wopl = new WoplFile(timbreLibrary);
        var woplBytes = wopl.GetRawWoplBytes(Assert.Fail);

        if (!expectedBytes.SequenceEqual(woplBytes))
            throw new InvalidOperationException("The default-initialised timbre library did not serialize to the expected bytes");

        player.OpenBankData(woplBytes);
        player.SetLoopEnabled(true);

        short[] buffer = new short[4096];
        player.Play(buffer);
    }
}
