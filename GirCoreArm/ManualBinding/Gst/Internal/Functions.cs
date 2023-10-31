using System;
using System.Runtime.InteropServices;

namespace GstSharp.ManualBinding.Gst.Internal;

public static class Functions
{
    /// <summary>
    ///     Calls native function gst_init.
    /// </summary>
    /// <param name="argc">Transfer ownership: Full Nullable: True</param>
    /// <param name="argv">Transfer ownership: Full Nullable: True</param>
    /// <returns>Transfer ownership: None Nullable: False</returns>
    [DllImport(ImportResolver.Library, EntryPoint = "gst_init")]
    public static extern void Init(ref int argc, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] string[] argv);

    /// <summary>
    ///     Calls native function gst_parse_launch.
    /// </summary>
    /// <param name="pipelineDescription">Transfer ownership: None Nullable: False</param>
    /// <param name="error"></param>
    /// <returns>Transfer ownership: None Nullable: False</returns>
    [DllImport(ImportResolver.Library, EntryPoint = "gst_parse_launch")]
    public static extern IntPtr ParseLaunch(IntPtr pipelineDescription, out IntPtr error);
}