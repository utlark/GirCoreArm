using System;
using System.Runtime.InteropServices;
using Gst;
using Gst.Internal;

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
    public static extern MessageOwnedHandle TimedPopFiltered(IntPtr bus, ClockTime timeout, MessageType types);
}