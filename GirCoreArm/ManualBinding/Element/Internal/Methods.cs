using System;
using System.Runtime.InteropServices;

namespace GstSharp.ManualBinding.Element.Internal;

public static class Methods
{
    /// <summary>
    ///     Calls native method gst_element_set_state.
    /// </summary>
    /// <param name="element">Transfer ownership: None Nullable: False</param>
    /// <param name="state">Transfer ownership: None Nullable: False</param>
    /// <returns>Transfer ownership: None Nullable: False</returns>
    [DllImport(ImportResolver.Library, EntryPoint = "gst_element_set_state")]
    public static extern StateChangeReturn SetState(IntPtr element, State state);

    /// <summary>
    ///     Calls native method gst_element_get_bus.
    /// </summary>
    /// <param name="element">Transfer ownership: None Nullable: False</param>
    /// <returns>Transfer ownership: Full Nullable: True</returns>
    [DllImport(ImportResolver.Library, EntryPoint = "gst_element_get_bus")]
    public static extern IntPtr GetBus(IntPtr element);
}