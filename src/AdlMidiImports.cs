/*
 * libADLMIDI is a free Software MIDI synthesizer library with OPL3 emulation
 *
 * Original ADLMIDI code: Copyright (c) 2010-2014 Joel Yliluoma <bisqwit@iki.fi>
 * ADLMIDI Library API:   Copyright (c) 2015-2020 Vitaly Novichkov <admin@wohlnet.ru>
 *
 * Library is based on the ADLMIDI, a MIDI player for Linux and Windows with OPL3 emulation:
 * http:// iki.fi/bisqwit/source/adlmidi.html
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

#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
#pragma warning disable IDE1006 // Naming Styles
namespace ADLMidi.NET
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    static class AdlMidiImports
    {
        public static readonly Version AdlMidiVersion = new Version(1, 4, 1);

        /// <summary>
        /// Sets number of emulated chips (from 1 to 100). Emulation of multiple chips extends polyphony limits
        /// @param device Instance of the library
        /// @param numChips Count of virtual chips to emulate
        /// @return 0 on success, &lt;0 when any error has occurred
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_setNumChips(IntPtr device, int numChips);

        /// <summary>
        /// Get current number of emulated chips
        /// @param device Instance of the library
        /// @return Count of working chip emulators
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_getNumChips(IntPtr device);

        /// <summary>
        /// Get obtained number of emulated chips
        /// @param device Instance of the library
        /// @return Count of working chip emulators
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_getNumChipsObtained(IntPtr device);

        /// <summary>
        /// Sets a number of the patches bank from 0 to N banks.
        ///
        /// Is recommended to call adl_reset() to apply changes to already-loaded file player or real-time.
        ///
        /// @param device Instance of the library
        /// @param bank Number of embedded bank
        /// @return 0 on success, &lt;0 when any error has occurred
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_setBank(IntPtr device, int bank);

        /// <summary>
        /// Returns total number of available banks
        /// @return Total number of available embedded banks
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_getBanksCount();

        /// <summary>
        /// Returns pointer to array of names of every bank
        /// @return Array of strings containing the name of every embedded bank
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern string[] adl_getBankNames();

        /// <summary>
        /// Reference to dynamic bank
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Bank
        {
            IntPtr Pointer0;
            IntPtr Pointer1;
            IntPtr Pointer2;
        }

        /// <summary>
        /// Version of the instrument data format
        /// </summary>
        public enum InstrumentVersion
        {
            ADLMIDI_InstrumentVersion = 0
        }

        #region Setup

        /// <summary>
        /// Pre-allocates a minimum number of bank slots. Returns the actual capacity
        /// @param device Instance of the library
        /// @param banks Count of bank slots to pre-allocate.
        /// @return actual capacity of reserved bank slots.
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_reserveBanks(IntPtr device, uint banks);
        /// <summary>
        /// Gets the bank designated by the identifier, optionally creating if it does not exist
        /// @param device Instance of the library
        /// @param id Identifier of dynamic bank
        /// @param flags Flags for dynamic bank access (BankAccessFlags)
        /// @param bank Reference to dynamic bank
        /// @return 0 on success, &lt;0 when any error has occurred
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_getBank(IntPtr device, ref BankId id, BankAccessFlags flags, out Bank bank);
        /// <summary>
        /// Gets the identifier of a bank
        /// @param device Instance of the library
        /// @param bank Reference to dynamic bank.
        /// @param id Identifier of dynamic bank
        /// @return 0 on success, &lt;0 when any error has occurred
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_getBankId(IntPtr device, ref Bank bank, out BankId id);
        /// <summary>
        /// Removes a bank
        /// @param device Instance of the library
        /// @param bank Reference to dynamic bank
        /// @return 0 on success, &lt;0 when any error has occurred
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_removeBank(IntPtr device, ref Bank bank);
        /// <summary>
        /// Gets the first bank
        /// @param device Instance of the library
        /// @param bank Reference to dynamic bank
        /// @return 0 on success, &lt;0 when any error has occurred
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_getFirstBank(IntPtr device, out Bank bank);
        /// <summary>
        /// Iterates to the next bank
        /// @param device Instance of the library
        /// @param bank Reference to dynamic bank
        /// @return 0 on success, &lt;0 when any error has occurred or end has been reached.
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_getNextBank(IntPtr device, out Bank bank);
        /// <summary>
        /// Gets the nth instrument in the bank [0..127]
        /// @param device Instance of the library
        /// @param bank Reference to dynamic bank
        /// @param index Index of the instrument
        /// @param ins Instrument entry
        /// @return 0 on success, &lt;0 when any error has occurred
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_getInstrument(IntPtr device, ref Bank bank, uint index, out Instrument ins);
        /// <summary>
        /// Sets the nth instrument in the bank [0..127]
        /// @param device Instance of the library
        /// @param bank Reference to dynamic bank
        /// @param index Index of the instrument
        /// @param ins Instrument structure pointer
        /// @return 0 on success, &lt;0 when any error has occurred
        ///
        /// This function allows to override an instrument on the fly
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_setInstrument(IntPtr device, ref Bank bank, uint index, ref Instrument ins);
        /// <summary>
        /// Loads the melodic or percussive part of the nth embedded bank
        /// @param device Instance of the library
        /// @param bank Reference to dynamic bank
        /// @param num Number of embedded bank to load into the current bank array
        /// @return 0 on success, &lt;0 when any error has occurred
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_loadEmbeddedBank(IntPtr device, ref Bank bank, int num);

        /// <summary>
        /// Sets number of 4-operator channels between all chips
        ///
        /// By default, it is automatically re-calculating every bank change.
        /// If you want to specify custom number of four operator channels,
        /// please call this function after bank change (adl_setBank() or adl_openBank()),
        /// otherwise, value will be overwritten by auto-calculated.
        /// If the count is specified as -1, an auto-calculated amount is used instead.
        ///
        /// @param device Instance of the library
        /// @param ops4 Count of four-op channels to allocate between all emulating chips
        /// @return 0 on success, &lt;0 when any error has occurred
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_setNumFourOpsChn(IntPtr device, int ops4);

        /// <summary>
        /// Get current total count of 4-operator channels between all chips
        /// @param device Instance of the library
        /// @return 0 on success, &lt;-1 when any error has occurred, but, -1 - "auto"
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_getNumFourOpsChn(IntPtr device);

        /// <summary>
        /// Get obtained total count of 4-operator channels between all chips
        /// @param device Instance of the library
        /// @return 0 on success, &lt;0 when any error has occurred
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_getNumFourOpsChnObtained(IntPtr device);

        /// <summary>
        /// Override Enable(1) or Disable(0) deep vibrato state. -1 - use bank default vibrato state
        /// @param device Instance of the library
        /// @param vibratoMode 0 - disabled, 1 - enabled
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_setHVibrato(IntPtr device, int vibratoMode);

        /// <summary>
        /// Get the deep vibrato state.
        /// @param device Instance of the library
        /// @return deep vibrato state on success, &lt;0 when any error has occurred
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_getHVibrato(IntPtr device);

        /// <summary>
        /// Override Enable(1) or Disable(0) deep tremolo state. -1 - use bank default tremolo state
        /// @param device Instance of the library
        /// @param tremoloMode 0 - disabled, 1 - enabled
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_setHTremolo(IntPtr device, int tremoloMode);

        /// <summary>
        /// Get the deep tremolo state.
        /// @param device Instance of the library
        /// @return deep tremolo state on success, &lt;0 when any error has occurred
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_getHTremolo(IntPtr device);

        /// <summary>
        /// Override Enable(1) or Disable(0) scaling of modulator volumes. -1 - use bank default scaling of modulator volumes
        /// @param device Instance of the library
        /// @param modulatorVolumeScaling 0 - disabled, 1 - enabled
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_setScaleModulators(IntPtr device, int modulatorVolumeScaling);

        /// <summary>
        /// Enable(1) or Disable(0) full-range brightness (MIDI CC74 used in XG music to filter result sounding) scaling
        ///
        /// By default, brightness affects sound between 0 and 64.
        /// When this option is enabled, the brightness will use full range from 0 up to 127.
        ///
        /// @param device Instance of the library
        /// @param fullRangeBrightness 0 - disabled, 1 - enabled
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_setFullRangeBrightness(IntPtr device, int fullRangeBrightness);

        /// <summary>
        /// Enable or disable built-in loop (built-in loop supports 'loopStart' and 'loopEnd' tags to loop specific part)
        /// @param device Instance of the library
        /// @param loopEnabled 0 - disabled, 1 - enabled
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_setLoopEnabled(IntPtr device, bool loopEnabled);

        /// <summary>
        /// Enable or disable soft panning with chip emulators
        /// @param device Instance of the library
        /// @param softPanEnabled 0 - disabled, 1 - enabled
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_setSoftPanEnabled(IntPtr device, bool softPanEnabled);

        /// <summary>
        /// Set different volume range model
        /// @param device Instance of the library
        /// @param volumeModel Volume model type (#VolumeModel)
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_setVolumeRangeModel(IntPtr device, VolumeModel volumeModel);

        /// <summary>
        /// Get the volume range model
        /// @param device Instance of the library
        /// @return volume model on success, &lt;0 when any error has occurred
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern VolumeModel adl_getVolumeRangeModel(IntPtr device);

        /// <summary>
        /// Load WOPL bank file from File System
        ///
        /// Is recommended to call adl_reset() to apply changes to already-loaded file player or real-time.
        ///
        /// @param device Instance of the library
        /// @param filePath Absolute or relative path to the WOPL bank file. UTF8 encoding is required, even on Windows.
        /// @return 0 on success, &lt;0 when any error has occurred
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_openBankFile(IntPtr device, string filePath);

        /// <summary>
        /// Load WOPL bank file from memory data
        ///
        /// Is recommended to call adl_reset() to apply changes to already-loaded file player or real-time.
        ///
        /// @param device Instance of the library
        /// @param mem Pointer to memory block where is raw data of WOPL bank file is stored
        /// @param size Size of given memory block
        /// @return 0 on success, &lt;0 when any error has occurred
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern unsafe int adl_openBankData(IntPtr device, byte* mem, uint size);

        /// <summary>
        /// Returns chip emulator name string
        /// @param device Instance of the library
        /// @return Understandable name of current OPL3 emulator
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern string adl_chipEmulatorName(IntPtr device);

        /// <summary>
        /// Switch the emulation core
        /// @param device Instance of the library
        /// @param emulator Type of emulator (#Emulator)
        /// @return 0 on success, &lt;0 when any error has occurred
        /// </summary>
        [DllImport("ADLMIDI.dll")]
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
        ///
        /// @param device Instance of the library
        /// @param enabled 0 - disabled, 1 - enabled
        /// @return 0 on success, &lt;0 when any error has occurred
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_setRunAtPcmRate(IntPtr device, bool enabled);

        /// <summary>
        /// Set 4-bit device identifier. Used by the SysEx processor.
        /// @param device Instance of the library
        /// @param id 4-bit device identifier
        /// @return 0 on success, &lt;0 when any error has occurred
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_setDeviceIdentifier(IntPtr device, uint id);

        /// <summary>
        /// @section Information
        /// </summary>

        /// <summary>
        /// Returns string which contains a version number
        /// @return String which contains a version of the library
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern string adl_linkedLibraryVersion();

        /// <summary>
        /// Returns structure which contains a version number of library
        /// @return Library version context structure which contains version number of the library
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern AdlVersion adl_linkedVersion();


        #endregion

        #region Error Info

        /// <summary>
        /// Returns string which contains last error message of initialization
        ///
        /// Don't use this function to get info on any function except of `adl_init`!
        /// Use `adl_errorInfo()` to get error information while workflow
        ///
        /// @return String with error message related to library initialization
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern string adl_errorString();

        /// <summary>
        /// Returns string which contains last error message on specific device
        /// @param device Instance of the library
        /// @return String with error message related to last function call returned non-zero value.
        /// </summary>
        [DllImport("ADLMIDI.dll")]
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
        ///
        /// @param sampleRate Output sample rate
        /// @return Instance of the library. If NULL was returned, check the `adl_errorString` message for more info.
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern IntPtr adl_init(long sampleRate);

        /// <summary>
        /// Close and delete ADLMIDI device
        /// @param device Instance of the library
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_close(IntPtr device);

        #endregion

        #region MIDI Sequencer 

        /// <summary>
        /// Load MIDI (or any other supported format) file from File System
        ///
        /// Available when library is built with built-in MIDI Sequencer support.
        ///
        /// @param device Instance of the library
        /// @param filePath Absolute or relative path to the music file. UTF8 encoding is required, even on Windows.
        /// @return 0 on success, &lt;0 when any error has occurred
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_openFile(IntPtr device, string filePath);

        /// <summary>
        /// Load MIDI (or any other supported format) file from memory data
        ///
        /// Available when library is built with built-in MIDI Sequencer support.
        ///
        /// @param device Instance of the library
        /// @param mem Pointer to memory block where is raw data of music file is stored
        /// @param size Size of given memory block
        /// @return 0 on success, &lt;0 when any error has occurred
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern unsafe int adl_openData(IntPtr device, byte* mem, uint size);

        /// <summary>
        /// Resets MIDI player (per-channel setup) into initial state
        /// @param device Instance of the library
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_reset(IntPtr device);

        /// <summary>
        /// Get total time length of current song
        ///
        /// Available when library is built with built-in MIDI Sequencer support.
        ///
        /// @param device Instance of the library
        /// @return Total song length in seconds
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern double adl_totalTimeLength(IntPtr device);

        /// <summary>
        /// Get loop start time if presented.
        ///
        /// Available when library is built with built-in MIDI Sequencer support.
        ///
        /// @param device Instance of the library
        /// @return Time position in seconds of loop start point, or -1 when file has no loop points
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern double adl_loopStartTime(IntPtr device);

        /// <summary>
        /// Get loop end time if present.
        ///
        /// Available when library is built with built-in MIDI Sequencer support.
        ///
        /// @param device Instance of the library
        /// @return Time position in seconds of loop end point, or -1 when file has no loop points
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern double adl_loopEndTime(IntPtr device);

        /// <summary>
        /// Get current time position in seconds
        ///
        /// Available when library is built with built-in MIDI Sequencer support.
        ///
        /// @param device Instance of the library
        /// @return Current time position in seconds
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern double adl_positionTell(IntPtr device);

        /// <summary>
        /// Jump to absolute time position in seconds
        ///
        /// Available when library is built with built-in MIDI Sequencer support.
        ///
        /// @param device Instance of the library
        /// @param seconds Destination time position in seconds to seek
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_positionSeek(IntPtr device, double seconds);

        /// <summary>
        /// Reset MIDI track position to begin
        ///
        /// Available when library is built with built-in MIDI Sequencer support.
        ///
        /// @param device Instance of the library
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_positionRewind(IntPtr device);

        /// <summary>
        /// Set tempo multiplier
        ///
        /// Available when library is built with built-in MIDI Sequencer support.
        ///
        /// @param device Instance of the library
        /// @param tempo Tempo multiplier value: 1.0 - original tempo, &gt;1 - play faster, &lt;1 - play slower
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_setTempo(IntPtr device, double tempo);

        /// <summary>
        /// Returns 1 if music position has reached end
        /// @param device Instance of the library
        /// @return 1 when end of sing has been reached, otherwise, 0 will be returned. &lt;0 is returned on any error
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_atEnd(IntPtr device);

        /// <summary>
        /// Returns the number of tracks of the current sequence
        /// @param device Instance of the library
        /// @return Count of tracks in the current sequence
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern UIntPtr adl_trackCount(IntPtr device);

        /// <summary>
        /// Sets options on a track of the current sequence
        /// @param device Instance of the library
        /// @param trackNumber Identifier of the designated track.
        /// @return 0 on success, &lt;0 when any error has occurred
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_setTrackOptions(IntPtr device, UIntPtr trackNumber, TrackOptions trackOptions);

        /// <summary>
        /// Handler of callback trigger events
        /// @param userData Pointer to user data (usually, context of something)
        /// @param trigger Value of the event which triggered this callback.
        /// @param track Identifier of the track which triggered this callback.
        /// </summary>
        public delegate void TriggerHandler(IntPtr userData, uint trigger, UIntPtr track);

        /// <summary>
        /// Defines a handler for callback trigger events
        /// @param device Instance of the library
        /// @param handler Handler to invoke from the sequencer when triggered, or NULL.
        /// @param userData Instance of the library
        /// @return 0 on success, &lt;0 when any error has occurred
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_setTriggerHandler(IntPtr device, TriggerHandler handler, IntPtr userData);

        #endregion

        #region Meta-Tags 

        /// <summary>
        /// Returns string which contains a music title
        /// @param device Instance of the library
        /// @return A string that contains music title
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern string adl_metaMusicTitle(IntPtr device);

        /// <summary>
        /// Returns string which contains a copyright string*
        /// @param device Instance of the library
        /// @return A string that contains copyright notice, otherwise NULL
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern string adl_metaMusicCopyright(IntPtr device);

        /// <summary>
        /// Returns count of available track titles
        ///
        /// NOTE: There are CAN'T be associated with channel in any of event or note hooks
        ///
        /// @param device Instance of the library
        /// @return Count of available MIDI tracks, otherwise NULL
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern UIntPtr adl_metaTrackTitleCount(IntPtr device);

        /// <summary>
        /// Get track title by index
        /// @param device Instance of the library
        /// @param index Index of the track to retrieve the title
        /// @return A string that contains track title, otherwise NULL.
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern string adl_metaTrackTitle(IntPtr device, UIntPtr index);

        /// <summary>
        /// Returns count of available markers
        /// @param device Instance of the library
        /// @return Count of available MIDI markers
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern UIntPtr adl_metaMarkerCount(IntPtr device);

        /// <summary>
        /// Returns the marker entry
        /// @param device Instance of the library
        /// @param index Index of the marker to retrieve it.
        /// @return MIDI Marker description structure.
        /// </summary>
        [DllImport("ADLMIDI.dll")]
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
        ///
        /// @param device Instance of the library
        /// @param sampleCount Count of samples (not frames!)
        /// @param out Pointer to output with 16-bit stereo PCM output
        /// @return Count of given samples, otherwise, 0 or when catching an error while playing
        /// </summary>
        [DllImport("ADLMIDI.dll")]
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
        ///
        /// @param device Instance of the library
        /// @param sampleCount Count of samples (not frames!)
        /// @param left Left channel buffer output (must be cast to byte array)
        /// @param right Right channel buffer output (must be cast to byte array)
        /// @param format Destination PCM format format context
        /// @return Count of given samples, otherwise, 0 or when catching an error while playing
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_playFormat(IntPtr device, int sampleCount, IntPtr left, IntPtr right, ref AudioFormat format);

        /// <summary>
        /// Generate PCM signed 16-bit stereo audio output without iteration of MIDI timers
        ///
        /// Use this function when you are using library as Real-Time MIDI synthesizer or with
        /// an external MIDI sequencer. You must to request the amount of samples which is equal
        /// to the delta between of MIDI event rows. One MIDI row is a group of MIDI events
        /// are having zero delta/delay between each other. When you are receiving events in
        /// real time, request the minimum possible delay value.
        ///
        /// Don't use count of frames, use instead count of samples. One frame is two samples.
        /// So, for example, if you want to take 10 frames, you must to request amount of 20 samples!
        ///
        /// @param device Instance of the library
        /// @param sampleCount
        /// @param out Pointer to output with 16-bit stereo PCM output
        /// @return Count of given samples, otherwise, 0 or when catching an error while playing
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern unsafe int adl_generate(IntPtr device, int sampleCount, short* sampleBuffer); // sampleBuffer = short[]

        /// <summary>
        /// Generate PCM stereo audio output in sample format declared by given context without iteration of MIDI timers
        ///
        /// Use this function when you are using library as Real-Time MIDI synthesizer or with
        /// an external MIDI sequencer. You must to request the amount of samples which is equal
        /// to the delta between of MIDI event rows. One MIDI row is a group of MIDI events
        /// are having zero delta/delay between each other. When you are receiving events in
        /// real time, request the minimum possible delay value.
        ///
        /// Don't use count of frames, use instead count of samples. One frame is two samples.
        /// So, for example, if you want to take 10 frames, you must to request amount of 20 samples!
        ///
        /// @param device Instance of the library
        /// @param sampleCount
        /// @param left Left channel buffer output (must be cast to byte array)
        /// @param right Right channel buffer output (must be cast to byte array)
        /// @param format Destination PCM format format context
        /// @return Count of given samples, otherwise, 0 or when catching an error while playing
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_generateFormat(IntPtr device, int sampleCount, IntPtr left, IntPtr right, ref AudioFormat format);

        /// <summary>
        /// Periodic tick handler.
        ///
        /// Notice: The function is provided to use it with Hardware OPL3 mode or for the purpose to iterate
        /// MIDI playback without of sound generation.
        ///
        /// DON'T USE IT TOGETHER WITH adl_play() and adl_playFormat() calls
        /// as there are all using this function internally!!!
        ///
        /// @param device Instance of the library
        /// @param seconds Previous delay. For the first moment, pass `0.0`
        /// @param granularity Minimum size of one MIDI tick in seconds.
        /// @return desired number of seconds until next call. Pass this value into `seconds` field in next time
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern double adl_tickEvents(IntPtr device, double seconds, double granularity);

        #endregion

        #region Real-Time MIDI 

        /// <summary>
        /// Force Off all notes on all channels
        /// @param device Instance of the library
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_panic(IntPtr device);

        /// <summary>
        /// Reset states of all controllers on all MIDI channels
        /// @param device Instance of the library
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_rt_resetState(IntPtr device);

        /// <summary>
        /// Turn specific MIDI note ON
        /// @param device Instance of the library
        /// @param channel Target MIDI channel [Between 0 and 16]
        /// @param note Note number to on [Between 0 and 127]
        /// @param velocity Velocity level [Between 0 and 127]
        /// @return 1 when note was successfully started, 0 when note was rejected for any reason.
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_rt_noteOn(IntPtr device, byte channel, byte note, byte velocity);

        /// <summary>
        /// Turn specific MIDI note OFF
        /// @param device Instance of the library
        /// @param channel Target MIDI channel [Between 0 and 16]
        /// @param note Note number to off [Between 0 and 127]
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_rt_noteOff(IntPtr device, byte channel, byte note);

        /// <summary>
        /// Set note after-touch
        /// @param device Instance of the library
        /// @param channel Target MIDI channel [Between 0 and 16]
        /// @param note Note number to affect by aftertouch event [Between 0 and 127]
        /// @param atVal After-Touch value [Between 0 and 127]
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_rt_noteAfterTouch(IntPtr device, byte channel, byte note, byte atVal);

        /// <summary>
        /// Set channel after-touch
        /// @param device Instance of the library
        /// @param channel Target MIDI channel [Between 0 and 16]
        /// @param atVal After-Touch level [Between 0 and 127]
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_rt_channelAfterTouch(IntPtr device, byte channel, byte atVal);

        /// <summary>
        /// Apply controller change
        /// @param device Instance of the library
        /// @param channel Target MIDI channel [Between 0 and 16]
        /// @param type Type of the controller [Between 0 and 255]
        /// @param value Value of the controller event [Between 0 and 127]
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_rt_controllerChange(IntPtr device, byte channel, byte type, byte value);

        /// <summary>
        /// Apply patch change
        /// @param device Instance of the library
        /// @param channel Target MIDI channel [Between 0 and 16]
        /// @param patch Patch number [Between 0 and 127]
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_rt_patchChange(IntPtr device, byte channel, byte patch);

        /// <summary>
        /// Apply pitch bend change
        /// @param device Instance of the library
        /// @param channel Target MIDI channel [Between 0 and 16]
        /// @param pitch 24-bit pitch bend value
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_rt_pitchBend(IntPtr device, byte channel, ushort pitch);

        /// <summary>
        /// Apply pitch bend change
        /// @param device Instance of the library
        /// @param channel Target MIDI channel [Between 0 and 16]
        /// @param msb MSB part of 24-bit pitch bend value
        /// @param lsb LSB part of 24-bit pitch bend value
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_rt_pitchBendML(IntPtr device, byte channel, byte msb, byte lsb);

        /// <summary>
        /// Change LSB of the bank number (Alias to CC-32 event)
        /// @param device Instance of the library
        /// @param channel Target MIDI channel [Between 0 and 16]
        /// @param lsb LSB value of the MIDI bank number
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_rt_bankChangeLSB(IntPtr device, byte channel, byte lsb);

        /// <summary>
        /// Change MSB of the bank (Alias to CC-0 event)
        /// @param device Instance of the library
        /// @param channel Target MIDI channel [Between 0 and 16]
        /// @param msb MSB value of the MIDI bank number
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_rt_bankChangeMSB(IntPtr device, byte channel, byte msb);

        /// <summary>
        /// Change bank by absolute signed value
        /// @param device Instance of the library
        /// @param channel Target MIDI channel [Between 0 and 16]
        /// @param bank Bank number as concatenated signed 16-bit value of MSB and LSB parts.
        /// </summary>

        [DllImport("ADLMIDI.dll")]
        public static extern void adl_rt_bankChange(IntPtr device, byte channel, short bank);

        /// <summary>
        /// Perform a system exclusive message
        /// @param device Instance of the library
        /// @param msg Raw SysEx message buffer (must begin with 0xF0 and end with 0xF7)
        /// @param size Size of given SysEx message buffer
        /// @return 1 when SysEx message was successfully processed, 0 when SysEx message was rejected for any reason
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern unsafe int adl_rt_systemExclusive(IntPtr device, byte* message, UIntPtr size);

        #endregion

        #region Hooks and debugging 

        /// <summary>
        /// Raw event callback
        /// @param user data Pointer to user data (usually, context of something)
        /// @param type MIDI event type
        /// @param subtype MIDI event sub-type (special events only)
        /// @param channel MIDI channel
        /// @param data Raw event data
        /// @param len Length of event data
        /// </summary>
        public delegate void RawEventHook(IntPtr userData, byte type, byte subType, byte channel, IntPtr data, UIntPtr length);

        /// <summary>
        /// Note on/off callback
        /// @param user data Pointer to user data (usually, context of something)
        /// @param adlChannel Chip channel where note was played
        /// @param note Note number [between 0 and 127]
        /// @param pressure Velocity level, or -1 when it's note off event
        /// @param bend Pitch bend offset value
        /// </summary>
        public delegate void NoteHook(IntPtr userData, int adlChannel, int note, int ins, int pressure, double bend);

        /// <summary>
        /// Set raw MIDI event hook
        /// @param device Instance of the library
        /// @param rawEventHook Pointer to the callback function which will be called on every MIDI event
        /// @param userData Pointer to user data which will be passed through the callback.
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_setRawEventHook(IntPtr device, RawEventHook rawEventHook, IntPtr userData);

        /// <summary>
        /// Set note hook
        /// @param device Instance of the library
        /// @param noteHook Pointer to the callback function which will be called on every noteOn MIDI event
        /// @param userData Pointer to user data which will be passed through the callback.
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_setNoteHook(IntPtr device, NoteHook noteHook, IntPtr userData);

#if false // Variadic delegate, ahh! Will worry about it if and when I need it.
        /// <summary>
        /// Debug messages callback
        /// @param userdata Pointer to user data (usually, context of something)
        /// @param fmt Format string output (in context of `printf()` standard function)
         /// </summary>
        typedef void (*DebugMessageHook)(IntPtr userdata, string fmt, ...);

        /// <summary>
        /// Set debug message hook
        /// @param device Instance of the library
        /// @param debugMessageHook Pointer to the callback function which will be called on every debug message
        /// @param userData Pointer to user data which will be passed through the callback.
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern void adl_setDebugMessageHook(IntPtr device, DebugMessageHook debugMessageHook, IntPtr userData);
#endif
        /// <summary>
        /// Get a textual description of the channel state. For display only.
        /// @param device Instance of the library
        /// @param text Destination char buffer for channel usage state. Every entry is assigned to the chip channel.
        /// @param attr Destination char buffer for additional attributes like MIDI channel number that uses this chip channel.
        /// @param size Size of given buffers (both text and attr are must have same size!)
        /// @return 0 on success, &lt;0 when any error has occurred
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
        /// To get the valid MIDI channel you will need to apply the & 0x0F mask to every value.
        /// </summary>
        [DllImport("ADLMIDI.dll")]
        public static extern int adl_describeChannels(IntPtr device, string text, string attr, UIntPtr size);
        #endregion
    }
}
#pragma warning restore CS0626 // Method, operator, or accessor is marked external and has no attributes on it
#pragma warning restore IDE1006 // Naming Styles
