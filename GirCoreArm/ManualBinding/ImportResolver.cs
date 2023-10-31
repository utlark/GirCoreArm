using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace GstSharp.ManualBinding;

internal static class ImportResolver
{
    public const string Library = "Gst";
    private const string WindowsLibraryName = "libgstreamer-1.0-0.dll";
    private const string LinuxLibraryName = "libgstreamer-1.0.so.0";
    private const string OsxLibraryName = "libgstreamer-1.0.0.dylib";

    private static IntPtr _targetLibraryPointer = IntPtr.Zero;

    public static void RegisterAsDllImportResolver()
    {
        NativeLibrary.SetDllImportResolver(typeof(ImportResolver).Assembly, Resolve);
    }

    private static IntPtr Resolve(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName != Library)
            return IntPtr.Zero;

        if (_targetLibraryPointer != IntPtr.Zero)
            return _targetLibraryPointer;

        var osDependentLibraryName = GetOsDependentLibraryName();
        _targetLibraryPointer = NativeLibrary.Load(osDependentLibraryName, assembly, searchPath);

        return _targetLibraryPointer;
    }

    private static string GetOsDependentLibraryName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return WindowsLibraryName;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return OsxLibraryName;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return LinuxLibraryName;

        throw new Exception("Unknown platform");
    }
}