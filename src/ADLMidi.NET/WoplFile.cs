using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SerdesNet;

namespace ADLMidi.NET;

/// <summary>
/// WOPL file
/// </summary>
public class WoplFile
{
    const string Magic = "WOPL3-BANK";

    /// <summary>
    /// The melodic instrument banks
    /// </summary>
    public IList<WoplBank> Melodic { get; } = new List<WoplBank>();

    /// <summary>
    /// The percussion instrument banks
    /// </summary>
    public IList<WoplBank> Percussion { get; } = new List<WoplBank>();

    /// <summary>
    /// Serialize or deserialize a WOPL file
    /// </summary>
    public static WoplFile Serdes(WoplFile w, ISerdes s)
    {
        if (s == null) throw new ArgumentNullException(nameof(s));
        w ??= new WoplFile();

        ushort melodicBanks = (ushort)w.Melodic.Count;
        ushort percussionBanks = (ushort)w.Percussion.Count;

        var magic = s.FixedLengthString(nameof(Magic), Magic, Magic.Length);
        if (magic != Magic)
            throw new InvalidOperationException("Magic string missing (invalid WOPL file)");

        s.Pad(1);

        w.Version = s.UInt16(nameof(Version), w.Version);
        melodicBanks = s.UInt16BE(nameof(melodicBanks), melodicBanks);
        percussionBanks = s.UInt16BE(nameof(percussionBanks), percussionBanks);
        w.GlobalFlags = s.EnumU8(nameof(GlobalFlags), w.GlobalFlags);
        w.VolumeModel = s.EnumU8(nameof(VolumeModel), w.VolumeModel);

        while (w.Melodic.Count < melodicBanks) w.Melodic.Add(new WoplBank());
        while (w.Percussion.Count < percussionBanks) w.Percussion.Add(new WoplBank());

        if (w.Version >= 2)
        {
            foreach (var bank in w.Melodic)
            {
                bank.Name = s.FixedLengthString(nameof(bank.Name), bank.Name, WoplBank.MaxNameLength);
                bank.Id = s.UInt16(nameof(bank.Id), bank.Id);
            }

            foreach (var bank in w.Percussion)
            {
                bank.Name = s.FixedLengthString(nameof(bank.Name), bank.Name, WoplBank.MaxNameLength);
                bank.Id = s.UInt16(nameof(bank.Id), bank.Id);
            }
        }

        // Load instruments (128 per bank)
        foreach (var bank in w.Melodic)
            s.List(nameof(w.Melodic), bank.Instruments, bank.Instruments.Length,
                (i2, w2, s2) => WoplInstrument.Serdes(i2, w2, s2, w.Version));

        foreach (var bank in w.Percussion)
            s.List(nameof(w.Percussion), bank.Instruments, bank.Instruments.Length,
                (i2, w2, s2) => WoplInstrument.Serdes(i2, w2, s2, w.Version));

        return w;
    }

    /// <summary>
    /// The WOPL file version
    /// </summary>
    public ushort Version { get; set; } = 3;

    /// <summary>
    /// The global flags
    /// </summary>
    public GlobalBankFlags GlobalFlags { get; set; }

    /// <summary>
    /// The volume model
    /// </summary>
    public VolumeModel VolumeModel { get; set; }

    /// <summary>
    /// Create a new WOPL file
    /// </summary>
    public WoplFile() { }

    /// <summary>
    /// Create a new WOPL file from a timbre library
    /// </summary>
    /// <param name="timbreLibrary"></param>
    public WoplFile(GlobalTimbreLibrary timbreLibrary)
    {
        Version = 3;
        GlobalFlags = GlobalBankFlags.DeepTremolo | GlobalBankFlags.DeepVibrato;
        VolumeModel = VolumeModel.Auto;

        Melodic.Add(new WoplBank { Id = 0, Name = "" });
        Percussion.Add(new WoplBank { Id = 0, Name = "" });

        for (int i = 0; i < timbreLibrary.Data.Count; i++)
        {
            var timbre = timbreLibrary.Data[i];
            WoplInstrument x =
                i < 128
                    ? Melodic[0].Instruments[i] ?? new WoplInstrument()
                    : Percussion[0].Instruments[i - 128 + 35] ?? new WoplInstrument();

            x.Name = "";
            x.NoteOffset1 = timbre.MidiPatchNumber;
            x.NoteOffset2 = timbre.MidiBankNumber;
            x.InstrumentMode = InstrumentMode.TwoOperator;
            x.FbConn1C0 = timbre.FeedbackConnection;
            x.Operator0 = timbre.Carrier;
            x.Operator1 = timbre.Modulation;
            x.Operator2 = Operator.Blank;
            x.Operator3 = Operator.Blank;

            if (i < 128)
                Melodic[0].Instruments[i] = x;
            else
                Percussion[0].Instruments[i - 128 + 35] = x;
        }
    }

    /// <summary>
    /// Serializes a WOPL file to a byte array
    /// </summary>
    /// <param name="assertionFailed">Method to call if an assertion about the data is violated.</param>
    /// <returns>The serialized byte array</returns>
    public byte[] GetRawWoplBytes(Action<string> assertionFailed)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        using var gbw = new WriterSerdes(bw, assertionFailed);
        Serdes(this, gbw);
        return ms.ToArray();
    }
}