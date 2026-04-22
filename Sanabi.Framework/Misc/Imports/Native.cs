using System.Runtime.InteropServices;

namespace Sanabi.Framework.Misc.Imports;

public static partial class NativeWin
{
    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint GetModuleHandle(string lpModuleName);

    public static nint? GetModuleHandleOrNullIfZero(string lpModuleName)
    {
        var handle = GetModuleHandle(lpModuleName);
        if (handle == nint.Zero)
            return null;

        return handle;
    }
}

/// <summary>
///     TODO: Test
/// </summary>
public static partial class NativeLinux
{
    public const int RTLD_NOLOAD = 4;

    [LibraryImport("libdl.so.2", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint dlopen(string? fileName, int flags);

    public static nint? DlopenOrNullIfZero(string? fileName, int flags)
    {
        var handle = dlopen(fileName, flags);
        if (handle == nint.Zero)
            return null;

        return handle;
    }
}
