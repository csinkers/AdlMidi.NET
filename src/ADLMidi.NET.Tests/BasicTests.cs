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
        using var player = AdlMidi.Init();

        GlobalTimbreLibrary timbreLibrary = new GlobalTimbreLibrary();
        var wopl = new WoplFile(timbreLibrary);
        var woplBytes = wopl.GetRawWoplBytes(Assert.Fail);

        player.OpenBankData(woplBytes);
        player.SetLoopEnabled(true);

        short[] buffer = new short[4096];
        player.Play(buffer);
    }
}
