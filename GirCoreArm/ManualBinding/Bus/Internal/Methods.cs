using System;
using System.Runtime.InteropServices;

namespace GstSharp.ManualBinding.Bus.Internal;

public static class Methods
{
    /// <summary>
    ///     Calls native method gst_bus_timed_pop_filtered.
    /// </summary>
    /// <param name="bus">Transfer ownership: None Nullable: False</param>
    /// <param name="timeout">Transfer ownership: None Nullable: False</param>
    /// <param name="types">Transfer ownership: None Nullable: False</param>
    /// <returns>Transfer ownership: Full Nullable: True</returns>
    [DllImport(ImportResolver.Library, EntryPoint = "gst_bus_timed_pop_filtered")]
    public static extern IntPtr TimedPopFiltered(IntPtr bus, ulong timeout, MessageType types);
}