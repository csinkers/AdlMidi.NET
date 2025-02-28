namespace ADLMidi.NET.Tests;

public class BasicTests
{
    [Fact]
    public void CreateDisposeTest()
    {
        using var player = AdlMidi.Init();
    }

    [Fact]
    public void PlayXmiTest()
    {
        using var player = AdlMidi.Init();
        byte[] xmiBytes = [0, 1, 2]; // TODO
        if (xmiBytes.Length == 0)
            return;

        GlobalTimbreLibrary timbreLibrary = new GlobalTimbreLibrary();
        var wopl = new WoplFile(timbreLibrary);
        var woplBytes = wopl.GetRawWoplBytes(Assert.Fail);

        player.OpenBankData(woplBytes);
        player.OpenData(xmiBytes);
        player.SetLoopEnabled(true);

        short[] buffer = new short[4096];
        player.Play(buffer);
    }
}
