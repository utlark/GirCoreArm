using System;
using System.Runtime.InteropServices;
using System.Text;

namespace GstSharp.ManualBinding.Gst;

public static class Functions
{
    public static Element.Element ParseLaunch(string pipelineDescription)
    {
        var parseLaunchResult = Internal.Functions.ParseLaunch(StringToPtrUtf8(pipelineDescription), out _);
        return new Element.Element(parseLaunchResult);
    }

    private static IntPtr StringToPtrUtf8(string? str)
    {
        if (str is null)
            return IntPtr.Zero;

        var bytes = Encoding.UTF8.GetBytes(str);
        var alloc = GLib.Internal.Functions.Malloc((uint)(bytes.Length + 1));
        Marshal.Copy(bytes, 0, alloc, bytes.Length);
        Marshal.WriteByte(alloc, bytes.Length, 0);

        return alloc;
    }
}