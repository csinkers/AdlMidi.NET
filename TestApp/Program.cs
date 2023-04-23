using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using ADLMidi.NET;
using SerdesNet;

namespace TestApp
{
    class Program
    {
        const int SampleRate = 44100;
        const string newWoplPath = @"C:\Depot\bb\ualbion\albion\DRIVERS\ALBISND_NEW.wopl";
        static readonly StringBuilder NoteWriter = new();
        static double Time = 0;
        
        static void Main()
        {
            // WoplFile realWopl = ReadWopl(@"C:\Depot\bb\ualbion\albion\DRIVERS\ALBISND.wopl");
            // WriteWopl(realWopl, @"C:\Depot\bb\ualbion\albion\DRIVERS\ALBISND_ROUNDTRIP.wopl");

            GlobalTimbreLibrary oplFile = ReadOpl(@"C:\Depot\bb\ualbion\albion\DRIVERS\ALBISND.OPL");
            WoplFile wopl = OplToWopl(oplFile);
            WriteWopl(wopl, newWoplPath);

            byte[] bankData = File.ReadAllBytes(newWoplPath);
            for (int i = 0; i <= 45; i++)
            {
                Console.WriteLine($"Dumping {i}");
                ExportAlbionSong(i, bankData);
                ExportSongEvents(i, bankData);
            }

            File.WriteAllText(@"C:\Depot\bb\ualbion\re\SongsPlayed.txt", NoteWriter.ToString());
        }

        static void ExportAlbionSong(int songId, byte[] bankData)
        {
            string path = $@"C:\Depot\bb\ualbion\Data\Exported\SONGS{songId/100}.XLD\{songId%100:D2}.xmi";
            if (!File.Exists(path))
                return;

            using var outputFile = new WavFile(
                $@"C:\Depot\bb\ualbion\re\Songs{songId/100}_{songId%100:D2}.wav",
                SampleRate, 2, 2);

            using var player = AdlMidi.Init();

            NoteWriter.AppendLine($"Song {songId}");
            player.SetNoteHook(NoteHook, IntPtr.Zero);
            player.OpenBankData(bankData);
            player.OpenFile(path);
            player.SetLoopEnabled(false);
            short[] bufferArray = new short[4096];
            long totalSamples = 0;
            for(;;)
            {
                Time = (double)totalSamples / (2 * SampleRate);
                int samplesWritten = player.Play(bufferArray);
                totalSamples += samplesWritten;

                if (samplesWritten <= 0)
                    break;

                var byteSpan = MemoryMarshal.Cast<short, byte>(new ReadOnlySpan<short>(bufferArray, 0, samplesWritten));
                outputFile.Write(byteSpan);
            }
            NoteWriter.AppendLine();
        }

        public static void ExportSongEvents(int songId, byte[] bankData)
        {
            string path = $@"C:\Depot\bb\ualbion\Data\Exported\SONGS{songId/100}.XLD\{songId%100:D2}.xmi";
            if (!File.Exists(path))
                return;

            using var player = AdlMidi.Init();

            NoteWriter.AppendLine($"Song {songId}");
            player.OpenBankData(bankData);
            player.SetNoteHook(NoteHook, IntPtr.Zero);
            player.OpenFile(path);
            player.SetLoopEnabled(false);
            double nextTime = 0;
            Time = 0;
            const double minTick = 1.0;
            while (player.AtEnd() == 0)
            {
                Time += nextTime;
                nextTime = player.TickEvents(nextTime, minTick);
            }

            NoteWriter.AppendLine();
        }

        static void NoteHook(IntPtr userdata, int adlchannel, int note, int ins, int pressure, double bend)
        {
            NoteWriter.AppendLine($"    {Time}: Chan{adlchannel} Note{note} Instrument{ins} Pressure{pressure} Bend{bend}");
        }

        static WoplFile OplToWopl(GlobalTimbreLibrary oplFile)
        {
            var wopl = new WoplFile
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
                x.InstrumentMode = InstrumentMode.TwoOperator;
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

            return wopl;
        }

        public sealed class WavFile : IDisposable
        {
            readonly FileStream _stream;
            readonly BinaryWriter _bw;
            readonly long _riffSizeOffset;
            readonly long _dataSizeOffset;
            uint _dataSize;

            public WavFile(string filename, uint sampleRate, ushort numChannels, ushort bytesPerSample)
            {
                _stream = File.Open(filename, FileMode.Create);
                _bw = new BinaryWriter(_stream);
                _bw.Write(Encoding.ASCII.GetBytes("RIFF")); // Container format chunk
                _riffSizeOffset = _stream.Position;
                _bw.Write(0); // Dummy write to start with, will be overwritten at the end.

                _bw.Write(Encoding.ASCII.GetBytes("WAVEfmt ")); // Subchunk1 (format metadata)
                _bw.Write(16);
                _bw.Write((ushort)1); // Format = Linear Quantisation
                _bw.Write(numChannels); // NumChannels
                _bw.Write(sampleRate); // SampleRate
                _bw.Write(sampleRate * numChannels * bytesPerSample); // ByteRate
                _bw.Write((ushort)(numChannels * bytesPerSample)); // BlockAlign
                _bw.Write((ushort)(bytesPerSample * 8)); // BitsPerSample

                _bw.Write(Encoding.ASCII.GetBytes("data")); // Subchunk2 (raw sample data)
                _dataSizeOffset = _stream.Position;
                _bw.Write(0); // Dummy write, will be overwritten at the end
            }

            public void Write(ReadOnlySpan<byte> buffer)
            {
                _bw.Write(buffer);
                _dataSize += (uint)buffer.Length;
            }

            public void Dispose()
            {
                var totalLength = _stream.Position; // Write actual length to container format chunk
                _stream.Position = _riffSizeOffset;
                _bw.Write((uint)(totalLength - 8));

                _stream.Position = _dataSizeOffset;
                _bw.Write(_dataSize);

                _bw.Dispose();
                _stream.Dispose();
            }
        }
/*
        static WoplFile ReadWopl(string filename)
        {
            using var stream2 = File.OpenRead(filename);
            using var br = new BinaryReader(stream2);
            return WoplFile.Serdes(null, new GenericBinaryReader(br, br.BaseStream.Length, Encoding.ASCII.GetString, Console.WriteLine));
        }
*/

        static void WriteWopl(WoplFile wopl, string filename)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            WoplFile.Serdes(wopl, new GenericBinaryWriter(bw, Encoding.ASCII.GetBytes, Console.WriteLine));
            byte[] bytes = ms.ToArray();
            File.WriteAllBytes(filename, bytes);
        }

        static GlobalTimbreLibrary ReadOpl(string filename)
        {
            using var stream = File.OpenRead(filename);
            using var br = new BinaryReader(stream);
            return GlobalTimbreLibrary.Serdes(null,
                new GenericBinaryReader(br, br.BaseStream.Length, Encoding.ASCII.GetString, Console.WriteLine));
        }
    }
}
