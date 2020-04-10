using System;

namespace ADLMidi.NET
{
    public class Bank
    {
        readonly IntPtr _device;
        AdlMidiImports.Bank _bank;

        internal Bank(IntPtr device, AdlMidiImports.Bank bank)
        {
            _device = device;
            _bank = bank;
        }

        public BankId GetBankId()
        {
            var result = AdlMidiImports.adl_getBankId(_device, ref _bank, out var id);
            if(result < 0)
                throw new InvalidOperationException();
            return id;
        }

        public int RemoveBank()
        {
            return AdlMidiImports.adl_removeBank(_device, ref _bank);
        }

        public Instrument GetInstrument(uint index)
        {
            var result = AdlMidiImports.adl_getInstrument(_device, ref _bank, index, out var instrument);
            if(result < 0)
                throw new InvalidOperationException();
            return instrument;
        }

        public int SetInstrument(uint index, ref Instrument ins)
        {
            return AdlMidiImports.adl_setInstrument(_device, ref _bank, index, ref ins);
        }

        public int LoadEmbeddedBank(int num)
        {
            return AdlMidiImports.adl_loadEmbeddedBank(_device, ref _bank, num);
        }
    }
}