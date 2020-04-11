using System;

namespace ADLMidi.NET
{
    public class MidiPlayer
    {
        readonly IntPtr _device;

        int Check(int result)
        {
            if (result >= 0)
                return result;

            var error = AdlMidiImports.adl_errorInfo(_device);
            throw new InvalidOperationException(error);
        }

        public MidiPlayer(IntPtr device)
        {
            _device = device;
        }

        public void OpenFile(string filePath) => Check(AdlMidiImports.adl_openFile(_device, filePath));
        public unsafe void OpenData(ReadOnlySpan<byte> data)
        {
            fixed (byte* ptr = data)
            {
                Check(AdlMidiImports.adl_openData(_device, ptr, (uint)data.Length));
            }
        }

        public void Close() => AdlMidiImports.adl_close(_device);
        public void Reset() => AdlMidiImports.adl_reset(_device);

        public int NumChips
        {
            get => AdlMidiImports.adl_getNumChips(_device);
            set => AdlMidiImports.adl_setNumChips(_device, value);
        }
        public int NumChipsObtained => AdlMidiImports.adl_getNumChipsObtained(_device);

        public int NumFourOpsChn
        {
            get => AdlMidiImports.adl_getNumFourOpsChn(_device);
            set => AdlMidiImports.adl_setNumFourOpsChn(_device, value);
        }
        public int NumFourOpsChnObtained => AdlMidiImports.adl_getNumFourOpsChnObtained(_device);

        public int Vibrato
        {
            get => AdlMidiImports.adl_getHVibrato(_device);
            set => AdlMidiImports.adl_setHVibrato(_device, value);
        }

        public int Tremolo
        {
            get => AdlMidiImports.adl_getHTremolo(_device);
            set => AdlMidiImports.adl_setHTremolo(_device, value);
        }

        public VolumeModel VolumeRangeModel
        {
            get => AdlMidiImports.adl_getVolumeRangeModel(_device);
            set => AdlMidiImports.adl_setVolumeRangeModel(_device, value);
        }

        public int ReserveBanks(uint banks) => AdlMidiImports.adl_reserveBanks(_device, banks);

        public Bank GetBank(BankId id, BankAccessFlags flags)
        {
            Check(AdlMidiImports.adl_getBank(_device, ref id, flags, out var bank));
            return new Bank(_device, bank);
        }
        public void SetBank(int bank) => Check(AdlMidiImports.adl_setBank(_device, bank));

        public Bank GetFirstBank()
        {
            Check(AdlMidiImports.adl_getFirstBank(_device, out var bank));
            return new Bank(_device, bank);
        }

        public Bank GetNextBank()
        {
            Check(AdlMidiImports.adl_getNextBank(_device, out var bank));
            return new Bank(_device, bank);
        }

        public void OpenBankFile(string filePath) => Check(AdlMidiImports.adl_openBankFile(_device, filePath));
        public unsafe void OpenBankData(ReadOnlySpan<byte> bankData)
        {
            fixed (byte* data = bankData)
            {
                Check(AdlMidiImports.adl_openBankData(_device, data, (uint)bankData.Length));
            }
        }

        public void SetScaleModulators(int modulatorVolumeScaling) => AdlMidiImports.adl_setScaleModulators(_device, modulatorVolumeScaling);
        public void SetFullRangeBrightness(int fullRangeBrightness) => AdlMidiImports.adl_setFullRangeBrightness(_device, fullRangeBrightness);
        public void SetLoopEnabled(bool loopEnabled) => AdlMidiImports.adl_setLoopEnabled(_device, loopEnabled);
        public void SetSoftPanEnabled(bool softPanEnabled) => AdlMidiImports.adl_setSoftPanEnabled(_device, softPanEnabled);
        public string ChipEmulatorName() => AdlMidiImports.adl_chipEmulatorName(_device);
        public void SwitchEmulator(Emulator emulator) => Check(AdlMidiImports.adl_switchEmulator(_device, emulator));
        public void SetRunAtPcmRate(bool enabled) => Check(AdlMidiImports.adl_setRunAtPcmRate(_device, enabled));
        public void SetDeviceIdentifier(uint id) => Check(AdlMidiImports.adl_setDeviceIdentifier(_device, id));

        public string ErrorInfo() => AdlMidiImports.adl_errorInfo(_device);
        public double TotalTimeLength() => AdlMidiImports.adl_totalTimeLength(_device);
        public double LoopStartTime() => AdlMidiImports.adl_loopStartTime(_device);
        public double LoopEndTime() => AdlMidiImports.adl_loopEndTime(_device);
        public double PositionTell() => AdlMidiImports.adl_positionTell(_device);
        public void PositionSeek(double seconds) => AdlMidiImports.adl_positionSeek(_device, seconds);
        public void PositionRewind() => AdlMidiImports.adl_positionRewind(_device);
        public void SetTempo(double tempo) => AdlMidiImports.adl_setTempo(_device, tempo);
        public int AtEnd() => AdlMidiImports.adl_atEnd(_device);
        public UIntPtr TrackCount() => AdlMidiImports.adl_trackCount(_device);
        public void SetTrackOptions(UIntPtr trackNumber, TrackOptions trackOptions) => Check(AdlMidiImports.adl_setTrackOptions(_device, trackNumber, trackOptions));
        // public int SetTriggerHandler(AdlMidiImports.TriggerHandler handler, IntPtr userData) => AdlMidiImports.adl_setTriggerHandler(_device, handler, userData);
        public string MetaMusicTitle() => AdlMidiImports.adl_metaMusicTitle(_device);
        public string MetaMusicCopyright() => AdlMidiImports.adl_metaMusicCopyright(_device);
        public UIntPtr MetaTrackTitleCount() => AdlMidiImports.adl_metaTrackTitleCount(_device);
        public string MetaTrackTitle(UIntPtr index) => AdlMidiImports.adl_metaTrackTitle(_device, index);
        public UIntPtr MetaMarkerCount() => AdlMidiImports.adl_metaMarkerCount(_device);
        public MarkerEntry MetaMarker(UIntPtr index) => AdlMidiImports.adl_metaMarker(_device, index);
        public unsafe int Play(Span<short> buffer)
        {
            fixed(short* p = buffer)
            {
                return Check(AdlMidiImports.adl_play(_device, buffer.Length, p));
            };
        }

        public int PlayFormat(int sampleCount, IntPtr left, IntPtr right, ref AudioFormat format) => Check(AdlMidiImports.adl_playFormat(_device, sampleCount, left, right, ref format));
        public unsafe int Generate(Span<short> buffer)
        {
            fixed (short* ptr = buffer)
            {
                return Check(AdlMidiImports.adl_generate(_device, buffer.Length, ptr));
            }
        }

        public int GenerateFormat(int sampleCount, IntPtr left, IntPtr right, ref AudioFormat format) => Check(AdlMidiImports.adl_generateFormat(_device, sampleCount, left, right, ref format));
        public double TickEvents(double seconds, double granularity) => AdlMidiImports.adl_tickEvents(_device, seconds, granularity);
        public void Panic() => AdlMidiImports.adl_panic(_device);

#if false
        public void RealTimeResetState() => AdlMidiImports.adl_rt_resetState(_device);
        public int RealTimeNoteOn(byte channel, byte note, byte velocity) => AdlMidiImports.adl_rt_noteOn(_device, channel, note, velocity);
        public void RealTimeNoteOff(byte channel, byte note) => AdlMidiImports.adl_rt_noteOff(_device, channel, note);
        public void RealTimeNoteAfterTouch(byte channel, byte note, byte atVal) => AdlMidiImports.adl_rt_noteAfterTouch(_device, channel, note, atVal);
        public void RealTimeChannelAfterTouch(byte channel, byte atVal) => AdlMidiImports.adl_rt_channelAfterTouch(_device, channel, atVal);
        public void RealTimeControllerChange(byte channel, byte type, byte value) => AdlMidiImports.adl_rt_controllerChange(_device, channel, type, value);
        public void RealTimePatchChange(byte channel, byte patch) => AdlMidiImports.adl_rt_patchChange(_device, channel, patch);
        public void RealTimePitchBend(byte channel, ushort pitch) => AdlMidiImports.adl_rt_pitchBend(_device, channel, pitch);
        public void RealTimePitchBendML(byte channel, byte msb, byte lsb) => AdlMidiImports.adl_rt_pitchBendML(_device, channel, msb, lsb);
        public void RealTimeBankChangeLSB(byte channel, byte lsb) => AdlMidiImports.adl_rt_bankChangeLSB(_device, channel, lsb);
        public void RealTimeBankChangeMSB(byte channel, byte msb) => AdlMidiImports.adl_rt_bankChangeMSB(_device, channel, msb);
        public void RealTimeBankChange(byte channel, short bank) => AdlMidiImports.adl_rt_bankChange(_device, channel, bank);
        public int RealTimeSystemExclusive(IntPtr message, UIntPtr size) => AdlMidiImports.adl_rt_systemExclusive(_device, message, size);
#endif

#if false
        public void SetRawEventHook(AdlMidiImports.RawEventHook rawEventHook, IntPtr userData) => AdlMidiImports.adl_setRawEventHook(_device, rawEventHook, userData);
        public void SetNoteHook(AdlMidiImports.NoteHook noteHook, IntPtr userData) => AdlMidiImports.adl_setNoteHook(_device, noteHook, userData);
        public int DescribeChannels(string text, string attr, UIntPtr size) => AdlMidiImports.adl_describeChannels(_device, text, attr, size);
#endif
    }
}

