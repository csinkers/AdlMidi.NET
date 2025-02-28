using System;

namespace ADLMidi.NET;

/// <summary>
/// Static class providing the entry point to the ADLMIDI library.
/// </summary>
public static class AdlMidi
{
    /// <summary>
    /// The default sample rate used by the OPL3 chip.
    /// </summary>
    public const int DefaultChipSampleRate = 49716;

    /// <summary>
    /// Gets the number of banks available.
    /// </summary>
    /// <returns>The number of banks available.</returns>
    public static int GetBankCount() => AdlMidiImports.adl_getBanksCount();

    /// <summary>
    /// Gets the names of the banks.
    /// </summary>
    /// <returns>An array of bank names.</returns>
    public static string[] GetBankNames() => AdlMidiImports.adl_getBankNames();

    /// <summary>
    /// Gets the version of the linked library as a string.
    /// </summary>
    /// <returns>The version of the linked library.</returns>
    public static string LinkedLibraryVersion() => AdlMidiImports.adl_linkedLibraryVersion();

    /// <summary>
    /// Gets the version of the linked library as a <see cref="Version"/> object.
    /// </summary>
    /// <returns>The version of the linked library.</returns>
    public static Version LinkedVersion()
    {
        var version = AdlMidiImports.adl_linkedVersion();
        return new Version(version.Major, version.Minor, version.Patch);
    }

    /// <summary>
    /// Gets the most recent error string from the ADLMIDI library.
    /// </summary>
    /// <returns>The error string.</returns>
    public static string ErrorString() => AdlMidiImports.adl_errorString();

    /// <summary>
    /// Initializes a new instance of the <see cref="MidiPlayer"/> class with the specified sample rate.
    /// </summary>
    /// <param name="sampleRate">The sample rate to use. Defaults to <see cref="DefaultChipSampleRate"/>.</param>
    /// <returns>A new instance of the <see cref="MidiPlayer"/> class.</returns>
    /// <exception cref="InvalidOperationException">Thrown if initialization fails.</exception>
    public static MidiPlayer Init(long sampleRate = DefaultChipSampleRate)
    {
        var midiPlayer = AdlMidiImports.adl_init(sampleRate);
        if (midiPlayer == IntPtr.Zero)
            throw new InvalidOperationException(ErrorString());

        return new MidiPlayer(midiPlayer);
    }
}