using System;
using System.Collections.Generic;
using SerdesNet;

namespace ADLMidi.NET
{
    public class WoplFile
    {
        const string Magic = "WOPL3-BANK";
        public IList<WoplBank> Melodic { get; } = new List<WoplBank>();
        public IList<WoplBank> Percussion { get; } = new List<WoplBank>();

        public static WoplFile Serdes(WoplFile w, ISerializer s)
        {
            w ??= new WoplFile();

            ushort melodicBanks = (ushort)w.Melodic.Count;
            ushort percussionBanks = (ushort)w.Percussion.Count;

            var magic = s.NullTerminatedString(nameof(Magic), Magic);
            if(magic != Magic)
                throw new InvalidOperationException("Magic string missing (invalid WOPL file)");

            w.Version = s.UInt16(nameof(Version), w.Version);
            s.PushVersion(w.Version);
            melodicBanks = s.UInt16BE(nameof(melodicBanks), melodicBanks);
            percussionBanks = s.UInt16BE(nameof(percussionBanks), percussionBanks);
            w.GlobalFlags = s.EnumU8(nameof(GlobalFlags), w.GlobalFlags);
            w.VolumeModel = s.EnumU8(nameof(VolumeModel), w.VolumeModel);

            while(w.Melodic.Count < melodicBanks) w.Melodic.Add(new WoplBank());
            while(w.Percussion.Count < percussionBanks) w.Percussion.Add(new WoplBank());

            if (w.Version >= 2)
            {
                foreach(var bank in w.Melodic)
                {
                    bank.Name = s.FixedLengthString(nameof(bank.Name), bank.Name, WoplBank.MaxNameLength);
                    bank.Id = s.UInt16(nameof(bank.Id), bank.Id);
                }

                foreach(var bank in w.Percussion)
                {
                    bank.Name = s.FixedLengthString(nameof(bank.Name), bank.Name, WoplBank.MaxNameLength);
                    bank.Id = s.UInt16(nameof(bank.Id), bank.Id);
                }
            }

            // Load instruments (128 per bank)
            foreach (var bank in w.Melodic)
                s.List(bank.Instruments, bank.Instruments.Length, WoplInstrument.Serdes);

            foreach (var bank in w.Percussion)
                s.List(bank.Instruments, bank.Instruments.Length, WoplInstrument.Serdes);

            s.PopVersion();
            return w;
        }

        public ushort Version { get; set; } = 3;
        public GlobalBankFlags GlobalFlags { get; set; }
        public VolumeModel VolumeModel { get; set; }
    }
}
