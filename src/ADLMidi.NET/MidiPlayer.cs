using System;

namespace ADLMidi.NET;

/// <summary>
/// Represents a MIDI player in the ADLMIDI library.
/// </summary>
public sealed class MidiPlayer : IDisposable
{
    readonly IntPtr _device;

    /// <summary>
    /// Checks the result of an operation and throws an exception if it failed.
    /// </summary>
    /// <param name="result">The result of the operation.</param>
    /// <returns>The result if it is non-negative.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the result is negative.</exception>
    int Check(int result)
    {
        if (result >= 0)
            return result;

        var error = AdlMidiImports.adl_errorInfo(_device);
        throw new InvalidOperationException(error);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MidiPlayer"/> class.
    /// </summary>
    /// <param name="device">The device pointer.</param>
    public MidiPlayer(IntPtr device)
    {
        _device = device;
    }

    /// <summary>
    /// Opens a MIDI file.
    /// </summary>
    /// <param name="filePath">The path to the MIDI file.</param>
    public void OpenFile(string filePath) => Check(AdlMidiImports.adl_openFile(_device, filePath));

    /// <summary>
    /// Opens MIDI data from a byte span.
    /// </summary>
    /// <param name="data">The MIDI data.</param>
    public unsafe void OpenData(ReadOnlySpan<byte> data)
    {
        fixed (byte* ptr = data)
        {
            Check(AdlMidiImports.adl_openData(_device, ptr, (uint)data.Length));
        }
    }

    /// <summary>
    /// Disposes the MIDI player, releasing any resources.
    /// </summary>
    public void Dispose()
    {
        if (_device != IntPtr.Zero)
            Close();
    }

    /// <summary>
    /// Closes the MIDI player.
    /// </summary>
    public void Close() => AdlMidiImports.adl_close(_device);

    /// <summary>
    /// Resets the MIDI player.
    /// </summary>
    public void Reset() => AdlMidiImports.adl_reset(_device);

    /// <summary>
    /// Gets or sets the number of OPL3 chips.
    /// </summary>
    public int NumChips
    {
        get => AdlMidiImports.adl_getNumChips(_device);
        set => AdlMidiImports.adl_setNumChips(_device, value);
    }

    /// <summary>
    /// Gets the number of OPL3 chips obtained.
    /// </summary>
    public int NumChipsObtained => AdlMidiImports.adl_getNumChipsObtained(_device);

    /// <summary>
    /// Gets or sets the number of four-operator channels.
    /// </summary>
    public int NumFourOpsChn
    {
        get => AdlMidiImports.adl_getNumFourOpsChn(_device);
        set => AdlMidiImports.adl_setNumFourOpsChn(_device, value);
    }

    /// <summary>
    /// Gets the number of four-operator channels obtained.
    /// </summary>
    public int NumFourOpsChnObtained => AdlMidiImports.adl_getNumFourOpsChnObtained(_device);

    /// <summary>
    /// Gets or sets the vibrato depth.
    /// </summary>
    public int Vibrato
    {
        get => AdlMidiImports.adl_getHVibrato(_device);
        set => AdlMidiImports.adl_setHVibrato(_device, value);
    }

    /// <summary>
    /// Gets or sets the tremolo depth.
    /// </summary>
    public int Tremolo
    {
        get => AdlMidiImports.adl_getHTremolo(_device);
        set => AdlMidiImports.adl_setHTremolo(_device, value);
    }

    /// <summary>
    /// Gets or sets the volume range model.
    /// </summary>
    public VolumeModel VolumeRangeModel
    {
        get => AdlMidiImports.adl_getVolumeRangeModel(_device);
        set => AdlMidiImports.adl_setVolumeRangeModel(_device, value);
    }

    /// <summary>
    /// Reserves a number of banks.
    /// </summary>
    /// <param name="banks">The number of banks to reserve.</param>
    /// <returns>An integer indicating the result of the operation.</returns>
    public int ReserveBanks(uint banks) => AdlMidiImports.adl_reserveBanks(_device, banks);

    /// <summary>
    /// Gets a bank by its identifier and access flags.
    /// </summary>
    /// <param name="id">The bank identifier.</param>
    /// <param name="flags">The access flags.</param>
    /// <returns>The <see cref="Bank"/> object.</returns>
    public Bank GetBank(BankId id, BankAccessFlags flags)
    {
        Check(AdlMidiImports.adl_getBank(_device, ref id, flags, out var bank));
        return new Bank(_device, bank);
    }

    /// <summary>
    /// Sets the current bank.
    /// </summary>
    /// <param name="bank">The bank number.</param>
    public void SetBank(int bank) => Check(AdlMidiImports.adl_setBank(_device, bank));

    /// <summary>
    /// Gets the first bank.
    /// </summary>
    /// <returns>The first <see cref="Bank"/> object.</returns>
    public Bank GetFirstBank()
    {
        Check(AdlMidiImports.adl_getFirstBank(_device, out var bank));
        return new Bank(_device, bank);
    }

    /// <summary>
    /// Gets the next bank.
    /// </summary>
    /// <returns>The next <see cref="Bank"/> object.</returns>
    public Bank GetNextBank()
    {
        Check(AdlMidiImports.adl_getNextBank(_device, out var bank));
        return new Bank(_device, bank);
    }

    /// <summary>
    /// Opens a bank file.
    /// </summary>
    /// <param name="filePath">The path to the bank file.</param>
    public void OpenBankFile(string filePath) => Check(AdlMidiImports.adl_openBankFile(_device, filePath));

    /// <summary>
    /// Opens bank data from a byte span.
    /// </summary>
    /// <param name="bankData">The bank data.</param>
    public unsafe void OpenBankData(ReadOnlySpan<byte> bankData)
    {
        fixed (byte* data = bankData)
        {
            Check(AdlMidiImports.adl_openBankData(_device, data, (uint)bankData.Length));
        }
    }

    /// <summary>
    /// Sets the scale modulators.
    /// </summary>
    /// <param name="modulatorVolumeScaling">The modulator volume scaling value.</param>
    public void SetScaleModulators(int modulatorVolumeScaling) => AdlMidiImports.adl_setScaleModulators(_device, modulatorVolumeScaling);

    /// <summary>
    /// Sets the full range brightness.
    /// </summary>
    /// <param name="fullRangeBrightness">The full range brightness value.</param>
    public void SetFullRangeBrightness(int fullRangeBrightness) => AdlMidiImports.adl_setFullRangeBrightness(_device, fullRangeBrightness);

    /// <summary>
    /// Enables or disables looping.
    /// </summary>
    /// <param name="loopEnabled">True to enable looping, false to disable.</param>
    public void SetLoopEnabled(bool loopEnabled) => AdlMidiImports.adl_setLoopEnabled(_device, loopEnabled);

    /// <summary>
    /// Enables or disables soft panning.
    /// </summary>
    /// <param name="softPanEnabled">True to enable soft panning, false to disable.</param>
    public void SetSoftPanEnabled(bool softPanEnabled) => AdlMidiImports.adl_setSoftPanEnabled(_device, softPanEnabled);

    /// <summary>
    /// Gets the name of the chip emulator.
    /// </summary>
    /// <returns>The name of the chip emulator.</returns>
    public string ChipEmulatorName() => AdlMidiImports.adl_chipEmulatorName(_device);

    /// <summary>
    /// Switches the emulator.
    /// </summary>
    /// <param name="emulator">The emulator to switch to.</param>
    public void SwitchEmulator(Emulator emulator) => Check(AdlMidiImports.adl_switchEmulator(_device, emulator));

    /// <summary>
    /// Sets whether to run at PCM rate.
    /// </summary>
    /// <param name="enabled">True to enable, false to disable.</param>
    public void SetRunAtPcmRate(bool enabled) => Check(AdlMidiImports.adl_setRunAtPcmRate(_device, enabled));

    /// <summary>
    /// Sets the device identifier.
    /// </summary>
    /// <param name="id">The device identifier.</param>
    public void SetDeviceIdentifier(uint id) => Check(AdlMidiImports.adl_setDeviceIdentifier(_device, id));

    /// <summary>
    /// Gets the error information.
    /// </summary>
    /// <returns>The error information.</returns>
    public string ErrorInfo() => AdlMidiImports.adl_errorInfo(_device);

    /// <summary>
    /// Gets the total time length of the MIDI file.
    /// </summary>
    /// <returns>The total time length in seconds.</returns>
    public double TotalTimeLength() => AdlMidiImports.adl_totalTimeLength(_device);

    /// <summary>
    /// Gets the loop start time of the MIDI file.
    /// </summary>
    /// <returns>The loop start time in seconds.</returns>
    public double LoopStartTime() => AdlMidiImports.adl_loopStartTime(_device);

    /// <summary>
    /// Gets the loop end time of the MIDI file.
    /// </summary>
    /// <returns>The loop end time in seconds.</returns>
    public double LoopEndTime() => AdlMidiImports.adl_loopEndTime(_device);

    /// <summary>
    /// Gets the current position in the MIDI file.
    /// </summary>
    /// <returns>The current position in seconds.</returns>
    public double PositionTell() => AdlMidiImports.adl_positionTell(_device);

    /// <summary>
    /// Seeks to a specific position in the MIDI file.
    /// </summary>
    /// <param name="seconds">The position in seconds.</param>
    public void PositionSeek(double seconds) => AdlMidiImports.adl_positionSeek(_device, seconds);

    /// <summary>
    /// Rewinds the MIDI file to the beginning.
    /// </summary>
    public void PositionRewind() => AdlMidiImports.adl_positionRewind(_device);

    /// <summary>
    /// Sets the tempo of the MIDI file.
    /// </summary>
    /// <param name="tempo">The tempo value.</param>
    public void SetTempo(double tempo) => AdlMidiImports.adl_setTempo(_device, tempo);

    /// <summary>
    /// Checks if the end of the MIDI file has been reached.
    /// </summary>
    /// <returns>1 if the end has been reached, 0 otherwise.</returns>
    public int AtEnd() => AdlMidiImports.adl_atEnd(_device);

    /// <summary>
    /// Gets the number of tracks in the MIDI file.
    /// </summary>
    /// <returns>The number of tracks.</returns>
    public UIntPtr TrackCount() => AdlMidiImports.adl_trackCount(_device);

    /// <summary>
    /// Sets the options for a specific track.
    /// </summary>
    /// <param name="trackNumber">The track number.</param>
    /// <param name="trackOptions">The track options.</param>
    public void SetTrackOptions(UIntPtr trackNumber, TrackOptions trackOptions) => Check(AdlMidiImports.adl_setTrackOptions(_device, trackNumber, trackOptions));

    // public int SetTriggerHandler(AdlMidiImports.TriggerHandler handler, IntPtr userData) => AdlMidiImports.adl_setTriggerHandler(_device, handler, userData);

    /// <summary>
    /// Gets the title of the MIDI music.
    /// </summary>
    /// <returns>The title of the MIDI music.</returns>
    public string MetaMusicTitle() => AdlMidiImports.adl_metaMusicTitle(_device);

    /// <summary>
    /// Gets the copyright information of the MIDI music.
    /// </summary>
    /// <returns>The copyright information.</returns>
    public string MetaMusicCopyright() => AdlMidiImports.adl_metaMusicCopyright(_device);

    /// <summary>
    /// Gets the number of track titles in the MIDI file.
    /// </summary>
    /// <returns>The number of track titles.</returns>
    public UIntPtr MetaTrackTitleCount() => AdlMidiImports.adl_metaTrackTitleCount(_device);

    /// <summary>
    /// Gets the title of a specific track.
    /// </summary>
    /// <param name="index">The index of the track.</param>
    /// <returns>The title of the track.</returns>
    public string MetaTrackTitle(UIntPtr index) => AdlMidiImports.adl_metaTrackTitle(_device, index);

    /// <summary>
    /// Gets the number of markers in the MIDI file.
    /// </summary>
    /// <returns>The number of markers.</returns>
    public UIntPtr MetaMarkerCount() => AdlMidiImports.adl_metaMarkerCount(_device);

    /// <summary>
    /// Gets a specific marker in the MIDI file.
    /// </summary>
    /// <param name="index">The index of the marker.</param>
    /// <returns>The <see cref="MarkerEntry"/> object.</returns>
    public MarkerEntry MetaMarker(UIntPtr index) => AdlMidiImports.adl_metaMarker(_device, index);

    /// <summary>
    /// Plays the MIDI data into a buffer.
    /// </summary>
    /// <param name="buffer">The buffer to play the data into.</param>
    /// <returns>The number of samples played.</returns>
    public unsafe int Play(Span<short> buffer)
    {
        fixed (short* p = buffer)
        {
            return Check(AdlMidiImports.adl_play(_device, buffer.Length, p));
        }
    }

    /// <summary>
    /// Plays the MIDI data with a specific format.
    /// </summary>
    /// <param name="sampleCount">The number of samples to play.</param>
    /// <param name="left">The left channel buffer.</param>
    /// <param name="right">The right channel buffer.</param>
    /// <param name="format">The audio format.</param>
    /// <returns>The number of samples played.</returns>
    public int PlayFormat(int sampleCount, IntPtr left, IntPtr right, ref AudioFormat format) => Check(AdlMidiImports.adl_playFormat(_device, sampleCount, left, right, ref format));

    /// <summary>
    /// Generates MIDI data into a buffer.
    /// </summary>
    /// <param name="buffer">The buffer to generate the data into.</param>
    /// <returns>The number of samples generated.</returns>
    public unsafe int Generate(Span<short> buffer)
    {
        fixed (short* ptr = buffer)
        {
            return Check(AdlMidiImports.adl_generate(_device, buffer.Length, ptr));
        }
    }

    /// <summary>
    /// Generates MIDI data with a specific format.
    /// </summary>
    /// <param name="sampleCount">The number of samples to generate.</param>
    /// <param name="left">The left channel buffer.</param>
    /// <param name="right">The right channel buffer.</param>
    /// <param name="format">The audio format.</param>
    /// <returns>The number of samples generated.</returns>
    public int GenerateFormat(int sampleCount, IntPtr left, IntPtr right, ref AudioFormat format) => Check(AdlMidiImports.adl_generateFormat(_device, sampleCount, left, right, ref format));

    /// <summary>
    /// Ticks the MIDI events by a specific amount of time.
    /// </summary>
    /// <param name="seconds">The amount of time in seconds.</param>
    /// <param name="granularity">The granularity of the tick.</param>
    /// <returns>The number of events ticked.</returns>
    public double TickEvents(double seconds, double granularity) => AdlMidiImports.adl_tickEvents(_device, seconds, granularity);

    /// <summary>
    /// Sends a panic signal to the MIDI player.
    /// </summary>
    public void Panic() => AdlMidiImports.adl_panic(_device);

    /// <summary>
    /// Sets a note hook callback.
    /// </summary>
    /// <param name="noteHook">The note hook callback.</param>
    /// <param name="userData">The user data to pass to the callback.</param>
    public void SetNoteHook(NoteHook noteHook, IntPtr userData) => AdlMidiImports.adl_setNoteHook(_device, noteHook, userData);

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
    public int DescribeChannels(string text, string attr, UIntPtr size) => AdlMidiImports.adl_describeChannels(_device, text, attr, size);
#endif
}