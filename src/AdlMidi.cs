using System;

namespace ADLMidi.NET
{
    public static class AdlMidi
    {
        public const int DefaultChipSampleRate = 49716;
        public static int GetBankCount() => AdlMidiImports.adl_getBanksCount();
        public static string[] GetBankNames() => AdlMidiImports.adl_getBankNames();
        public static string LinkedLibraryVersion() => AdlMidiImports.adl_linkedLibraryVersion();

        public static Version LinkedVersion()
        {
            var version = AdlMidiImports.adl_linkedVersion();
            return new Version(version.Major, version.Minor, version.Patch);
        }

        public static string ErrorString() => AdlMidiImports.adl_errorString();
        public static MidiPlayer Init(long sampleRate = DefaultChipSampleRate)
        {
            var midiPlayer = AdlMidiImports.adl_init(sampleRate);
            if (midiPlayer == IntPtr.Zero)
                throw new InvalidOperationException(ErrorString());

            return new MidiPlayer(midiPlayer);
        }
    }
}