using System.Reflection;
using System.Runtime.InteropServices;

namespace ADLMidi.NET.Tests;

public class DllImportFixture : IDisposable
{
    private static readonly object _resolverLock = new();
    private static bool _resolverSet;

    // Workaround for the terrible support of native dependencies with ProjectReferences.
    // SetDllImportResolver can only be called once per assembly per process, so we
    // guard it across multiple test classes sharing this fixture.
    public DllImportFixture()
    {
        lock (_resolverLock)
        {
            if (_resolverSet) return;
            _resolverSet = true;
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
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    filename = string.Equals(Path.GetExtension(name), ".DYLIB", StringComparison.OrdinalIgnoreCase)
                        ? name
                        : name + ".dylib";
                }
                else throw new PlatformNotSupportedException();

                var fullPath = Path.Combine(root, "runtimes", runtime, "native", filename);
                return File.Exists(fullPath)
                    ? NativeLibrary.Load(fullPath)
                    : IntPtr.Zero;
            });
        }
    }

    public void Dispose() { }
}
