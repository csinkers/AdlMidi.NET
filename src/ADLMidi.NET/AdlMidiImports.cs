/*
 * libADLMIDI is a free Software MIDI synthesizer library with OPL3 emulation
 *
 * Original ADLMIDI code: Copyright (c) 2010-2014 Joel Yliluoma <bisqwit@iki.fi>
 * ADLMIDI Library API:   Copyright (c) 2015-2020 Vitaly Novichkov <admin@wohlnet.ru>
 *
 * Library is based on the ADLMIDI, a MIDI player for Linux and Windows with OPL3 emulation:
 * http://iki.fi/bisqwit/source/adlmidi.html
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http:// www.gnu.org/licenses/>.
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ADLMidi.NET;

/// <summary>
/// Raw event callback
/// </summary>
/// <param name="userData">Pointer to user data (usually, context of something)</param>
/// <param name="type">MIDI event type</param>
/// <param name="subType">MIDI event subtype (special events only)</param>
/// <param name="channel">MIDI channel</param>
/// <param name="data">Raw event data</param>
/// <param name="length">Length of event data</param>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public delegate void RawEventHook(IntPtr userData, byte type, byte subType, byte channel, IntPtr data, UIntPtr length);

/// <summary>
/// Note on/off callback
/// </summary>
/// <param name="userData">Pointer to user data (usually, context of something)</param>
/// <param name="adlChannel">Chip channel where note was played</param>
/// <param name="note">Note number [between 0 and 127]</param>
/// <param name="ins">Instrument number</param>
/// <param name="pressure">Velocity level, or -1 when it's note off event</param>
/// <param name="bend">Pitch bend offset value</param>
public delegate void NoteHook(IntPtr userData, int adlChannel, int note, int ins, int pressure, double bend);

internal static class AdlMidiImports
{
    const string LibraryName = "libadlmidi";
    // public static readonly Version AdlMidiVersion = new(1, 4, 1);

    /// <summary>
    /// Sets number of emulated chips (from 1 to 100). Emulation of multiple chips extends polyphony limits
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="numChips">Count of virtual chips to emulate</param>
    /// <returns>0 on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern int adl_setNumChips(IntPtr device, int numChips);

    /// <summary>
    /// Get current number of emulated chips
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <returns>Count of working chip emulators</returns>
    [DllImport(LibraryName)]
    public static extern int adl_getNumChips(IntPtr device);

    /// <summary>
    /// Get obtained number of emulated chips
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <returns>Count of working chip emulators</returns>
    [DllImport(LibraryName)]
    public static extern int adl_getNumChipsObtained(IntPtr device);

    /// <summary>
    /// Sets a number of the patches bank from 0 to N banks.
    /// It is recommended to call adl_reset() to apply changes to already-loaded file player or real-time.
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="bank">Number of embedded bank</param>
    /// <returns>0 on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern int adl_setBank(IntPtr device, int bank);

    /// <summary>
    /// Returns total number of available banks
    /// </summary>
    /// <returns>Total number of available embedded banks</returns>
    [DllImport(LibraryName)]
    public static extern int adl_getBanksCount();

    /// <summary>
    /// Returns pointer to array of names of every bank
    /// </summary>
    /// <returns>Array of strings containing the name of every embedded bank</returns>
    [DllImport(LibraryName)]
    public static extern string[] adl_getBankNames();

    /// <summary>
    /// Reference to dynamic bank
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Bank
    {
        public IntPtr Pointer0;
        public IntPtr Pointer1;
        public IntPtr Pointer2;
    }

    #region Setup

    /// <summary>
    /// Pre-allocates a minimum number of bank slots. Returns the actual capacity
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="banks">Count of bank slots to pre-allocate.</param>
    /// <returns>actual capacity of reserved bank slots.</returns>
    [DllImport(LibraryName)]
    public static extern int adl_reserveBanks(IntPtr device, uint banks);

    /// <summary>
    /// Gets the bank designated by the identifier, optionally creating if it does not exist
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="id">Identifier of dynamic bank</param>
    /// <param name="flags">Flags for dynamic bank access (BankAccessFlags)</param>
    /// <param name="bank">Reference to dynamic bank</param>
    /// <returns>0 on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern int adl_getBank(IntPtr device, ref BankId id, BankAccessFlags flags, out Bank bank);

    /// <summary>
    /// Gets the identifier of a bank
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="bank">Reference to dynamic bank.</param>
    /// <param name="id">Identifier of dynamic bank</param>
    /// <returns>0 on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern int adl_getBankId(IntPtr device, ref Bank bank, out BankId id);

    /// <summary>
    /// Removes a bank
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="bank">Reference to dynamic bank</param>
    /// <returns>0 on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern int adl_removeBank(IntPtr device, ref Bank bank);

    /// <summary>
    /// Gets the first bank
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="bank">Reference to dynamic bank</param>
    /// <returns>0 on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern int adl_getFirstBank(IntPtr device, out Bank bank);

    /// <summary>
    /// Iterates to the next bank
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="bank">Reference to dynamic bank</param>
    /// <returns>0 on success, &lt;0 when any error has occurred or end has been reached.</returns>
    [DllImport(LibraryName)]
    public static extern int adl_getNextBank(IntPtr device, out Bank bank);

    /// <summary>
    /// Gets the nth instrument in the bank [0..127]
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="bank">Reference to dynamic bank</param>
    /// <param name="index">Index of the instrument</param>
    /// <param name="ins">Instrument entry</param>
    /// <returns>0 on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern int adl_getInstrument(IntPtr device, ref Bank bank, uint index, out Instrument ins);

    /// <summary>
    /// Sets the nth instrument in the bank [0..127]
    ///
    /// This function allows to override an instrument on the fly
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="bank">Reference to dynamic bank</param>
    /// <param name="index">Index of the instrument</param>
    /// <param name="ins">Instrument structure pointer</param>
    /// <returns>0 on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern int adl_setInstrument(IntPtr device, ref Bank bank, uint index, ref Instrument ins);

    /// <summary>
    /// Loads the melodic or percussive part of the nth embedded bank
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="bank">Reference to dynamic bank</param>
    /// <param name="num">Number of embedded bank to load into the current bank array</param>
    /// <returns>0 on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern int adl_loadEmbeddedBank(IntPtr device, ref Bank bank, int num);

    /// <summary>
    /// Sets number of 4-operator channels between all chips
    ///
    /// By default, it is automatically re-calculating every bank change.
    /// If you want to specify custom number of four operator channels,
    /// please call this function after bank change (adl_setBank() or adl_openBank()),
    /// otherwise, value will be overwritten by auto-calculated.
    /// If the count is specified as -1, an auto-calculated amount is used instead.
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="ops4">Count of four-op channels to allocate between all emulating chips</param>
    /// <returns>0 on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern int adl_setNumFourOpsChn(IntPtr device, int ops4);

    /// <summary>
    /// Get current total count of 4-operator channels between all chips
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <returns>0 on success, &lt;-1 when any error has occurred, but, -1 - "auto"</returns>
    [DllImport(LibraryName)]
    public static extern int adl_getNumFourOpsChn(IntPtr device);

    /// <summary>
    /// Get obtained total count of 4-operator channels between all chips
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <returns>0 on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern int adl_getNumFourOpsChnObtained(IntPtr device);

    /// <summary>
    /// Override Enable(1) or Disable(0) deep vibrato state. -1 - use bank default vibrato state
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="vibratoMode">0 - disabled, 1 - enabled</param>
    [DllImport(LibraryName)]
    public static extern void adl_setHVibrato(IntPtr device, int vibratoMode);

    /// <summary>
    /// Get the deep vibrato state.
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <returns>deep vibrato state on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern int adl_getHVibrato(IntPtr device);

    /// <summary>
    /// Override Enable(1) or Disable(0) deep tremolo state. -1 - use bank default tremolo state
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="tremoloMode">0 - disabled, 1 - enabled</param>
    [DllImport(LibraryName)]
    public static extern void adl_setHTremolo(IntPtr device, int tremoloMode);

    /// <summary>
    /// Get the deep tremolo state.
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <returns>deep tremolo state on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern int adl_getHTremolo(IntPtr device);

    /// <summary>
    /// Override Enable(1) or Disable(0) scaling of modulator volumes. -1 - use bank default scaling of modulator volumes
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="modulatorVolumeScaling">0 - disabled, 1 - enabled</param>
    [DllImport(LibraryName)]
    public static extern void adl_setScaleModulators(IntPtr device, int modulatorVolumeScaling);

    /// <summary>
    /// Enable(1) or Disable(0) full-range brightness (MIDI CC74 used in XG music to filter result sounding) scaling
    ///
    /// By default, brightness affects sound between 0 and 64.
    /// When this option is enabled, the brightness will use full range from 0 up to 127.
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="fullRangeBrightness">0 - disabled, 1 - enabled</param>
    [DllImport(LibraryName)]
    public static extern void adl_setFullRangeBrightness(IntPtr device, int fullRangeBrightness);

    /// <summary>
    /// Enable or disable built-in loop (built-in loop supports 'loopStart' and 'loopEnd' tags to loop specific part)
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="loopEnabled">0 - disabled, 1 - enabled</param>
    [DllImport(LibraryName)]
    public static extern void adl_setLoopEnabled(IntPtr device, bool loopEnabled);

    /// <summary>
    /// Enable or disable soft panning with chip emulators
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="softPanEnabled">0 - disabled, 1 - enabled</param>
    [DllImport(LibraryName)]
    public static extern void adl_setSoftPanEnabled(IntPtr device, bool softPanEnabled);

    /// <summary>
    /// Set different volume range model
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="volumeModel">Volume model type (#VolumeModel)</param>
    [DllImport(LibraryName)]
    public static extern void adl_setVolumeRangeModel(IntPtr device, VolumeModel volumeModel);

    /// <summary>
    /// Get the volume range model
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <returns>volume model on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern VolumeModel adl_getVolumeRangeModel(IntPtr device);

    /// <summary>
    /// Load WOPL bank file from File System
    ///
    /// Is recommended to call adl_reset() to apply changes to already-loaded file player or real-time.
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="filePath">Absolute or relative path to the WOPL bank file. UTF8 encoding is required, even on Windows.</param>
    /// <returns>0 on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern int adl_openBankFile(IntPtr device, string filePath);

    /// <summary>
    /// Load WOPL bank file from memory data
    ///
    /// Is recommended to call adl_reset() to apply changes to already-loaded file player or real-time.
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="mem">Pointer to memory block where is raw data of WOPL bank file is stored</param>
    /// <param name="size">Size of given memory block</param>
    /// <returns>0 on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern unsafe int adl_openBankData(IntPtr device, byte* mem, uint size);

    /// <summary>
    /// Returns chip emulator name string
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <returns>Understandable name of current OPL3 emulator</returns>
    [DllImport(LibraryName)]
    public static extern string adl_chipEmulatorName(IntPtr device);

    /// <summary>
    /// Switch the emulation core
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="emulator">Type of emulator (#Emulator)</param>
    /// <returns>0 on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern int adl_switchEmulator(IntPtr device, Emulator emulator);

    /// <summary>
    /// Library version context
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AdlVersion
    {
        public ushort Major;
        public ushort Minor;
        public ushort Patch;
    }

    /// <summary>
    /// Run emulator with PCM rate to reduce CPU usage on slow devices.
    ///
    /// May decrease sounding accuracy on some chip emulators.
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="enabled">0 - disabled, 1 - enabled</param>
    /// <returns>0 on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern int adl_setRunAtPcmRate(IntPtr device, bool enabled);

    /// <summary>
    /// Set 4-bit device identifier. Used by the SysEx processor.
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="id">4-bit device identifier</param>
    /// <returns>0 on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern int adl_setDeviceIdentifier(IntPtr device, uint id);

    #endregion

    #region Information

    /// <summary>
    /// Returns string which contains a version number
    /// </summary>
    /// <returns>String which contains a version of the library</returns>
    [DllImport(LibraryName)]
    public static extern string adl_linkedLibraryVersion();

    /// <summary>
    /// Returns structure which contains a version number of library
    /// </summary>
    /// <returns>Library version context structure which contains version number of the library</returns>
    [DllImport(LibraryName)]
    public static extern AdlVersion adl_linkedVersion();

    #endregion

    #region Error Info

    /// <summary>
    /// Returns string which contains last error message of initialization
    ///
    /// Don't use this function to get info on any function except of `adl_init`!
    /// Use `adl_errorInfo()` to get error information while workflow
    /// </summary>
    /// <returns>String with error message related to library initialization</returns>
    [DllImport(LibraryName)]
    public static extern string adl_errorString();

    /// <summary>
    /// Returns string which contains last error message on specific device
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <returns>String with error message related to last function call returned non-zero value.</returns>
    [DllImport(LibraryName)]
    public static extern string adl_errorInfo(IntPtr device);

    #endregion

    #region Initialization 

    /// <summary>
    /// Initialize ADLMIDI Player device
    ///
    /// Tip 1: You can initialize multiple instances and run them in parallel
    /// Tip 2: Library is NOT thread-safe, therefore don't use same instance in different threads or use mutexes
    /// Tip 3: Changing of sample rate on the fly is not supported. Re-create the instance again.
    /// Top 4: To generate output in OPL chip native sample rate, please initialize it with sample rate value as `ChipSampleRate`
    /// </summary>
    /// <param name="sampleRate">Output sample rate</param>
    /// <returns>Instance of the library. If NULL was returned, check the `adl_errorString` message for more info.</returns>
    [DllImport(LibraryName)]
    public static extern IntPtr adl_init(long sampleRate);

    /// <summary>
    /// Close and delete ADLMIDI device
    /// </summary>
    /// <param name="device">Instance of the library</param>
    [DllImport(LibraryName)]
    public static extern void adl_close(IntPtr device);

    #endregion

    #region MIDI Sequencer 

    /// <summary>
    /// Load MIDI (or any other supported format) file from File System
    ///
    /// Available when library is built with built-in MIDI Sequencer support.
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="filePath">Absolute or relative path to the music file. UTF8 encoding is required, even on Windows.</param>
    /// <returns>0 on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern int adl_openFile(IntPtr device, string filePath);

    /// <summary>
    /// Load MIDI (or any other supported format) file from memory data
    ///
    /// Available when library is built with built-in MIDI Sequencer support.
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="mem">Pointer to memory block where is raw data of music file is stored</param>
    /// <param name="size">Size of given memory block</param>
    /// <returns>0 on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern unsafe int adl_openData(IntPtr device, byte* mem, uint size);

    /// <summary>
    /// Resets MIDI player (per-channel setup) into initial state
    /// </summary>
    /// <param name="device">Instance of the library</param>
    [DllImport(LibraryName)]
    public static extern void adl_reset(IntPtr device);

    /// <summary>
    /// Get total time length of current song
    ///
    /// Available when library is built with built-in MIDI Sequencer support.
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <returns>Total song length in seconds</returns>
    [DllImport(LibraryName)]
    public static extern double adl_totalTimeLength(IntPtr device);

    /// <summary>
    /// Get loop start time if presented.
    ///
    /// Available when library is built with built-in MIDI Sequencer support.
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <returns>Time position in seconds of loop start point, or -1 when file has no loop points</returns>
    [DllImport(LibraryName)]
    public static extern double adl_loopStartTime(IntPtr device);

    /// <summary>
    /// Get loop end time if present.
    ///
    /// Available when library is built with built-in MIDI Sequencer support.
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <returns>Time position in seconds of loop end point, or -1 when file has no loop points</returns>
    [DllImport(LibraryName)]
    public static extern double adl_loopEndTime(IntPtr device);

    /// <summary>
    /// Get current time position in seconds
    ///
    /// Available when library is built with built-in MIDI Sequencer support.
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <returns>Current time position in seconds</returns>
    [DllImport(LibraryName)]
    public static extern double adl_positionTell(IntPtr device);

    /// <summary>
    /// Jump to absolute time position in seconds
    ///
    /// Available when library is built with built-in MIDI Sequencer support.
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="seconds">Destination time position in seconds to seek</param>
    [DllImport(LibraryName)]
    public static extern void adl_positionSeek(IntPtr device, double seconds);

    /// <summary>
    /// Reset MIDI track position to begin
    ///
    /// Available when library is built with built-in MIDI Sequencer support.
    /// </summary>
    /// <param name="device">Instance of the library</param>
    [DllImport(LibraryName)]
    public static extern void adl_positionRewind(IntPtr device);

    /// <summary>
    /// Set tempo multiplier
    ///
    /// Available when library is built with built-in MIDI Sequencer support.
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="tempo">Tempo multiplier value: 1.0 - original tempo, &gt;1 - play faster, &lt;1 - play slower</param>
    [DllImport(LibraryName)]
    public static extern void adl_setTempo(IntPtr device, double tempo);

    /// <summary>
    /// Returns 1 if music position has reached end
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <returns>1 when end of sing has been reached, otherwise, 0 will be returned. &lt;0 is returned on any error</returns>
    [DllImport(LibraryName)]
    public static extern int adl_atEnd(IntPtr device);

    /// <summary>
    /// Returns the number of tracks of the current sequence
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <returns>Count of tracks in the current sequence</returns>
    [DllImport(LibraryName)]
    public static extern UIntPtr adl_trackCount(IntPtr device);

    /// <summary>
    /// Sets options on a track of the current sequence
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="trackNumber">Identifier of the designated track.</param>
    /// <param name="trackOptions">Options for the designated track.</param>
    /// <returns>0 on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern int adl_setTrackOptions(IntPtr device, UIntPtr trackNumber, TrackOptions trackOptions);

    /// <summary>
    /// Handler of callback trigger events
    /// </summary>
    /// <param name="userData">Pointer to user data (usually, context of something)</param>
    /// <param name="trigger">Value of the event which triggered this callback.</param>
    /// <param name="track">Identifier of the track which triggered this callback.</param>
    public delegate void TriggerHandler(IntPtr userData, uint trigger, UIntPtr track);

    /// <summary>
    /// Defines a handler for callback trigger events
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="handler">Handler to invoke from the sequencer when triggered, or NULL.</param>
    /// <param name="userData">Instance of the library</param>
    /// <returns>0 on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern int adl_setTriggerHandler(IntPtr device, TriggerHandler handler, IntPtr userData);

    #endregion

    #region Meta-Tags 

    /// <summary>
    /// Returns string which contains a music title
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <returns>A string that contains music title</returns>
    [DllImport(LibraryName)]
    public static extern string adl_metaMusicTitle(IntPtr device);

    /// <summary>
    /// Returns string which contains a copyright string*
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <returns>A string that contains copyright notice, otherwise NULL</returns>
    [DllImport(LibraryName)]
    public static extern string adl_metaMusicCopyright(IntPtr device);

    /// <summary>
    /// Returns count of available track titles
    ///
    /// NOTE: There CAN'T be associated with channel in any of event or note hooks
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <returns>Count of available MIDI tracks, otherwise NULL</returns>
    [DllImport(LibraryName)]
    public static extern UIntPtr adl_metaTrackTitleCount(IntPtr device);

    /// <summary>
    /// Get track title by index
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="index">Index of the track to retrieve the title</param>
    /// <returns>A string that contains track title, otherwise NULL.</returns>
    [DllImport(LibraryName)]
    public static extern string adl_metaTrackTitle(IntPtr device, UIntPtr index);

    /// <summary>
    /// Returns count of available markers
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <returns>Count of available MIDI markers</returns>
    [DllImport(LibraryName)]
    public static extern UIntPtr adl_metaMarkerCount(IntPtr device);

    /// <summary>
    /// Returns the marker entry
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="index">Index of the marker to retrieve it.</param>
    /// <returns>MIDI Marker description structure.</returns>
    [DllImport(LibraryName)]
    public static extern MarkerEntry adl_metaMarker(IntPtr device, UIntPtr index);

    #endregion

    #region Audio output Generation 

    /// <summary>
    /// Generate PCM signed 16-bit stereo audio output and iterate MIDI timers
    ///
    /// Use this function when you are playing a MIDI file loaded by `adl_openFile` or `adl_openData`
    /// when using the built-in MIDI sequencer.
    ///
    /// Don't use count of frames, instead use the count of samples. One frame is two samples.
    /// So, for example, if you want to take 10 frames, you must request 20 samples!
    ///
    /// Available when library is built with built-in MIDI Sequencer support.
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="sampleCount">Count of samples (not frames!)</param>
    /// <param name="sampleBuffer">Pointer to output with 16-bit stereo PCM output</param>
    /// <returns>Count of given samples, otherwise, 0 or when catching an error while playing</returns>
    [DllImport(LibraryName)]
    public static extern unsafe int adl_play(IntPtr device, int sampleCount, short* sampleBuffer);

    /// <summary>
    /// Generate PCM stereo audio output in sample format declared by given context and iterate MIDI timers
    ///
    /// Use this function when you are playing MIDI file loaded by `adl_openFile` or `adl_openData`
    /// when using the built-in MIDI sequencer.
    ///
    /// Don't use count of frames, instead use the count of samples. One frame is two samples.
    /// So, for example, if you want to take 10 frames, you must request 20 samples!
    ///
    /// Available when library is built with built-in MIDI Sequencer support.
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="sampleCount">Count of samples (not frames!)</param>
    /// <param name="left">Left channel buffer output (must be cast to byte array)</param>
    /// <param name="right">Right channel buffer output (must be cast to byte array)</param>
    /// <param name="format">Destination PCM format context</param>
    /// <returns>Count of given samples, otherwise, 0 or when catching an error while playing</returns>
    [DllImport(LibraryName)]
    public static extern int adl_playFormat(IntPtr device, int sampleCount, IntPtr left, IntPtr right, ref AudioFormat format);

    /// <summary>
    /// Generate PCM signed 16-bit stereo audio output without iteration of MIDI timers
    ///
    /// Use this function when you are using library as Real-Time MIDI synthesizer or with
    /// an external MIDI sequencer. You must request the amount of samples which is equal
    /// to the delta between of MIDI event rows. One MIDI row is a group of MIDI events
    /// are having zero delta/delay between each other. When you are receiving events in
    /// real time, request the minimum possible delay value.
    ///
    /// Don't use count of frames, use instead count of samples. One frame is two samples.
    /// So, for example, if you want to take 10 frames, you must request amount of 20 samples!
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="sampleCount">The number of samples to generate</param>
    /// <param name="sampleBuffer">Pointer to output with 16-bit stereo PCM output</param>
    /// <returns>Count of given samples, otherwise, 0 or when catching an error while playing</returns>
    [DllImport(LibraryName)]
    public static extern unsafe int adl_generate(IntPtr device, int sampleCount, short* sampleBuffer); // sampleBuffer = short[]

    /// <summary>
    /// Generate PCM stereo audio output in sample format declared by given context without iteration of MIDI timers
    ///
    /// Use this function when you are using library as Real-Time MIDI synthesizer or with
    /// an external MIDI sequencer. You must request the amount of samples which is equal
    /// to the delta between of MIDI event rows. One MIDI row is a group of MIDI events
    /// are having zero delta/delay between each other. When you are receiving events in
    /// real time, request the minimum possible delay value.
    ///
    /// Don't use count of frames, use instead count of samples. One frame is two samples.
    /// So, for example, if you want to take 10 frames, you must request amount of 20 samples!
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="sampleCount">The number of samples to generate</param>
    /// <param name="left">Left channel buffer output (must be cast to byte array)</param>
    /// <param name="right">Right channel buffer output (must be cast to byte array)</param>
    /// <param name="format">Destination PCM format context</param>
    /// <returns>Count of given samples, otherwise, 0 or when catching an error while playing</returns>
    [DllImport(LibraryName)]
    public static extern int adl_generateFormat(IntPtr device, int sampleCount, IntPtr left, IntPtr right, ref AudioFormat format);

    /// <summary>
    /// Periodic tick handler.
    ///
    /// Notice: The function is provided to use it with Hardware OPL3 mode or for the purpose of iterating
    /// MIDI playback without sound generation.
    ///
    /// DON'T USE IT TOGETHER WITH adl_play() and adl_playFormat() calls
    /// as there are all using this function internally!!!
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="seconds">Previous delay. For the first moment, pass `0.0`</param>
    /// <param name="granularity">Minimum size of one MIDI tick in seconds.</param>
    /// <returns>desired number of seconds until next call. Pass this value into `seconds` field in next time</returns>
    [DllImport(LibraryName)]
    public static extern double adl_tickEvents(IntPtr device, double seconds, double granularity);

    #endregion

    #region Real-Time MIDI 

    /// <summary>
    /// Force Off all notes on all channels
    /// </summary>
    /// <param name="device">Instance of the library</param>
    [DllImport(LibraryName)]
    public static extern void adl_panic(IntPtr device);

    /// <summary>
    /// Reset states of all controllers on all MIDI channels
    /// </summary>
    /// <param name="device">Instance of the library</param>
    [DllImport(LibraryName)]
    public static extern void adl_rt_resetState(IntPtr device);

    /// <summary>
    /// Turn specific MIDI note ON
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="channel">Target MIDI channel [Between 0 and 16]</param>
    /// <param name="note">Note number to on [Between 0 and 127]</param>
    /// <param name="velocity">Velocity level [Between 0 and 127]</param>
    /// <returns>1 when note was successfully started, 0 when note was rejected for any reason.</returns>
    [DllImport(LibraryName)]
    public static extern int adl_rt_noteOn(IntPtr device, byte channel, byte note, byte velocity);

    /// <summary>
    /// Turn specific MIDI note OFF
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="channel">Target MIDI channel [Between 0 and 16]</param>
    /// <param name="note">Note number to off [Between 0 and 127]</param>
    [DllImport(LibraryName)]
    public static extern void adl_rt_noteOff(IntPtr device, byte channel, byte note);

    /// <summary>
    /// Set note after-touch
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="channel">Target MIDI channel [Between 0 and 16]</param>
    /// <param name="note">Note number to affect by aftertouch event [Between 0 and 127]</param>
    /// <param name="atVal">After-Touch value [Between 0 and 127]</param>
    [DllImport(LibraryName)]
    public static extern void adl_rt_noteAfterTouch(IntPtr device, byte channel, byte note, byte atVal);

    /// <summary>
    /// Set channel after-touch
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="channel">Target MIDI channel [Between 0 and 16]</param>
    /// <param name="atVal">After-Touch level [Between 0 and 127]</param>
    [DllImport(LibraryName)]
    public static extern void adl_rt_channelAfterTouch(IntPtr device, byte channel, byte atVal);

    /// <summary>
    /// Apply controller change
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="channel">Target MIDI channel [Between 0 and 16]</param>
    /// <param name="type">Type of the controller [Between 0 and 255]</param>
    /// <param name="value">Value of the controller event [Between 0 and 127]</param>
    [DllImport(LibraryName)]
    public static extern void adl_rt_controllerChange(IntPtr device, byte channel, byte type, byte value);

    /// <summary>
    /// Apply patch change
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="channel">Target MIDI channel [Between 0 and 16]</param>
    /// <param name="patch">Patch number [Between 0 and 127]</param>
    [DllImport(LibraryName)]
    public static extern void adl_rt_patchChange(IntPtr device, byte channel, byte patch);

    /// <summary>
    /// Apply pitch bend change
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="channel">Target MIDI channel [Between 0 and 16]</param>
    /// <param name="pitch">24-bit pitch bend value</param>
    [DllImport(LibraryName)]
    public static extern void adl_rt_pitchBend(IntPtr device, byte channel, ushort pitch);

    /// <summary>
    /// Apply pitch bend change
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="channel">Target MIDI channel [Between 0 and 16]</param>
    /// <param name="msb">MSB part of 24-bit pitch bend value</param>
    /// <param name="lsb">LSB part of 24-bit pitch bend value</param>
    [DllImport(LibraryName)]
    public static extern void adl_rt_pitchBendML(IntPtr device, byte channel, byte msb, byte lsb);

    /// <summary>
    /// Change LSB of the bank number (Alias to CC-32 event)
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="channel">Target MIDI channel [Between 0 and 16]</param>
    /// <param name="lsb">LSB value of the MIDI bank number</param>
    [DllImport(LibraryName)]
    public static extern void adl_rt_bankChangeLSB(IntPtr device, byte channel, byte lsb);

    /// <summary>
    /// Change MSB of the bank (Alias to CC-0 event)
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="channel">Target MIDI channel [Between 0 and 16]</param>
    /// <param name="msb">MSB value of the MIDI bank number</param>
    [DllImport(LibraryName)]
    public static extern void adl_rt_bankChangeMSB(IntPtr device, byte channel, byte msb);

    /// <summary>
    /// Change bank by absolute signed value
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="channel">Target MIDI channel [Between 0 and 16]</param>
    /// <param name="bank">Bank number as concatenated signed 16-bit value of MSB and LSB parts.</param>

    [DllImport(LibraryName)]
    public static extern void adl_rt_bankChange(IntPtr device, byte channel, short bank);

    /// <summary>
    /// Perform a system exclusive message
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="message">Raw SysEx message buffer (must begin with 0xF0 and end with 0xF7)</param>
    /// <param name="size">Size of given SysEx message buffer</param>
    /// <returns>1 when SysEx message was successfully processed, 0 when SysEx message was rejected for any reason</returns>
    [DllImport(LibraryName)]
    public static extern unsafe int adl_rt_systemExclusive(IntPtr device, byte* message, UIntPtr size);

    #endregion

    #region Hooks and debugging 

    /// <summary>
    /// Set raw MIDI event hook
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="rawEventHook">Pointer to the callback function which will be called on every MIDI event</param>
    /// <param name="userData">Pointer to user data which will be passed through the callback.</param>
    [DllImport(LibraryName)]
    public static extern void adl_setRawEventHook(IntPtr device, RawEventHook rawEventHook, IntPtr userData);

    /// <summary>
    /// Set note hook
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="noteHook">Pointer to the callback function which will be called on every noteOn MIDI event</param>
    /// <param name="userData">Pointer to user data which will be passed through the callback.</param>
    [DllImport(LibraryName)]
    public static extern void adl_setNoteHook(IntPtr device, NoteHook noteHook, IntPtr userData);

#if false // Variadic delegate, ahh! Will worry about it if and when I need it.
        /// <summary>
        /// Debug messages callback
        /// </summary>
        /// <param name="userdata">Pointer to user data (usually, context of something)</param>
        /// <param name="fmt">Format string output (in context of `printf()` standard function)</param>
        typedef void (*DebugMessageHook)(IntPtr userdata, string fmt, ...);

        /// <summary>
        /// Set debug message hook
        /// </summary>
        /// <param name="device">Instance of the library</param>
        /// <param name="debugMessageHook">Pointer to the callback function which will be called on every debug message</param>
        /// <param name="userData">Pointer to user data which will be passed through the callback.</param>
        [DllImport(LibraryName)]
        public static extern void adl_setDebugMessageHook(IntPtr device, DebugMessageHook debugMessageHook, IntPtr userData);
#endif
    /// <summary>
    /// Get a textual description of the channel state. For display only.
    ///
    /// Every character in the `text` buffer means the type of usage:
    /// ```
    ///  `-` - channel is unused (free)
    ///  `+` - channel is used by two-operator voice
    ///  `#` - channel is used by four-operator voice
    ///  `@` - channel is used to play automatic arpeggio on chip channels overflow
    ///  `r` - rhythm-mode channel note
    /// ```
    ///
    /// The `attr` field receives the MIDI channel from which the chip channel is used.
    /// To get the valid MIDI channel you will need to apply the &amp; 0x0F mask to every value.
    /// </summary>
    /// <param name="device">Instance of the library</param>
    /// <param name="text">Destination char buffer for channel usage state. Every entry is assigned to the chip channel.</param>
    /// <param name="attr">Destination char buffer for additional attributes like MIDI channel number that uses this chip channel.</param>
    /// <param name="size">Size of given buffers (both text and attr are must have same size!)</param>
    /// <returns>0 on success, &lt;0 when any error has occurred</returns>
    [DllImport(LibraryName)]
    public static extern int adl_describeChannels(IntPtr device, string text, string attr, UIntPtr size);

    #endregion
}
