using System.Runtime.InteropServices;

namespace ADLMidi.NET
{
    /// <summary>
    /// Sound output format context
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AudioFormat
    {
        public SampleType Type; // type of sample
        public uint ContainerSize; // size in bytes of the storage type
        public uint SampleOffset; // distance in bytes between consecutive samples
    }
}
