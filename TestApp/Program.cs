using System;
using System.IO;
using System.Text;
using ADLMidi.NET;
using SerdesNet;

namespace TestApp
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            WoplFile realWopl;
            GlobalTimbreLibrary oplFile;

            {
                using var stream2 = File.OpenRead(@"C:\Depot\bb\ualbion\albion\DRIVERS\ALBISND.wopl");
                using var br2 = new BinaryReader(stream2);
                realWopl = WoplFile.Serdes(null, new GenericBinaryReader(br2, br2.BaseStream.Length, Encoding.ASCII.GetString, Console.WriteLine));
            }

            {
                using var ms = new MemoryStream();
                using var bw = new BinaryWriter(ms);
                WoplFile.Serdes(realWopl, new GenericBinaryWriter(bw, Encoding.ASCII.GetBytes, Console.WriteLine));
                byte[] roundTripped = ms.ToArray();
                File.WriteAllBytes(@"C:\Depot\bb\ualbion\albion\DRIVERS\ALBISND_ROUNDTRIP.wopl", roundTripped);
            }

            {
                using var stream = File.OpenRead(@"C:\Depot\bb\ualbion\albion\DRIVERS\ALBISND.OPL");
                using var br = new BinaryReader(stream);
                oplFile = GlobalTimbreLibrary.Serdes(null,
                    new GenericBinaryReader(br, br.BaseStream.Length, Encoding.ASCII.GetString, Console.WriteLine));
            }

            WoplFile wopl = new WoplFile
            {
                Version = 3,
                GlobalFlags = GlobalBankFlags.DeepTremolo | GlobalBankFlags.DeepVibrato,
                VolumeModel = VolumeModel.Auto
            };

            wopl.Melodic.Add(new WoplBank { Id = 0, Name = "" });
            wopl.Percussion.Add(new WoplBank { Id = 0, Name = "" });

            for(int i = 0; i < oplFile.Data.Count; i++)
            {
                var timbre = oplFile.Data[i];
                WoplInstrument x =
                    i < 128
                        ? wopl.Melodic[0].Instruments[i] ?? new WoplInstrument()
                        : wopl.Percussion[0].Instruments[i - 128 + 35] ?? new WoplInstrument();

                x.Name = "";
                x.NoteOffset1 = timbre.MidiPatchNumber;
                x.NoteOffset2 = timbre.MidiBankNumber;
                x.Flags = InstrumentFlags.TwoOp;
                x.FbConn1C0 = timbre.FeedbackConnection;
                x.Operator0 = timbre.Carrier;
                x.Operator1 = timbre.Modulation;
                x.Operator2 = Operator.Blank;
                x.Operator3 = Operator.Blank;

                if (i < 128)
                    wopl.Melodic[0].Instruments[i] = x;
                else
                    wopl.Percussion[0].Instruments[i - 128 + 35] = x;
            }

            {
                using var ms = new MemoryStream();
                using var bw = new BinaryWriter(ms);
                WoplFile.Serdes(wopl, new GenericBinaryWriter(bw, Encoding.ASCII.GetBytes, Console.WriteLine));
                byte[] bankData = ms.ToArray();
                File.WriteAllBytes(@"C:\Depot\bb\ualbion\albion\DRIVERS\ALBISND_NEW.wopl", bankData);

                var player = AdlMidi.Init();
                fixed (byte* bytes = bankData)
                {
                    player.OpenBankData((IntPtr)bytes, (uint)bankData.Length);
                }

                player.OpenFile(@"C:\Depot\bb\ualbion\Data\Exported\SONGS0.XLD\03.xmi");
                // IntPtr buffer = IntPtr.Zero;
                // player.Play(200000, buffer);
                player.Close();
            }
        }
    }
}
