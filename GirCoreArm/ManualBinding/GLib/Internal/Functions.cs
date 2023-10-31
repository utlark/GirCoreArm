using System;
using System.Runtime.InteropServices;

namespace GstSharp.ManualBinding.GLib.Internal;

public static class Functions
{
    /// <summary>
    ///     Calls native function g_malloc.
    /// </summary>
    /// <param name="nBytes">Transfer ownership: None Nullable: False</param>
    /// <returns>Transfer ownership: None Nullable: True</returns>
    [DllImport(ImportResolver.Library, EntryPoint = "g_malloc")]
    public static extern IntPtr Malloc(nuint nBytes);
}