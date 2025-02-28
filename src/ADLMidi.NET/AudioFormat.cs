using System.Runtime.InteropServices;

namespace ADLMidi.NET;

/// <summary>
/// Sound output format context
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct AudioFormat
{
    /// <summary>
    /// Type of sample
    /// </summary>
    public SampleType Type;

    /// <summary>
    /// Size in bytes of the storage type
    /// </summary>
    public uint ContainerSize;

    /// <summary>
    /// Distance in bytes between consecutive samples
    /// </summary>
    public uint SampleOffset;
}
