using System.Reflection;
using System.Runtime.InteropServices;

namespace ADLMidi.NET.Tests;

public class DllImportFixture : IDisposable
{
    // Workaround for the terrible support of native dependencies with ProjectReferences
    public DllImportFixture()
    {
        NativeLibrary.SetDllImportResolver(
            typeof(AdlMidi).Assembly,
            (name, assembly, path) =>
            {
                var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

                string filename;
                string runtime = RuntimeInformation.RuntimeIdentifier;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    filename = string.Equals(Path.GetExtension(name), ".DLL", StringComparison.OrdinalIgnoreCase)
                        ? name
                        : name + ".dll";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    filename = string.Equals(Path.GetExtension(name), ".SO", StringComparison.OrdinalIgnoreCase)
                        ? name
                        : name + ".so";
                }
                else throw new PlatformNotSupportedException();

                var fullPath = Path.Combine(root, "runtimes", runtime, "native", filename);
                return File.Exists(fullPath)
                    ? NativeLibrary.Load(fullPath)
                    : IntPtr.Zero;
            });
    }

    public void Dispose() { }
}
