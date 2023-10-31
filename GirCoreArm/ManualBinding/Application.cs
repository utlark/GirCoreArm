using System;
using GstSharp.ManualBinding.Gst.Internal;

namespace GstSharp.ManualBinding;

public static class Application
{
    public static void Init()
    {
        var argc = 0;
        Functions.Init(ref argc, Array.Empty<string>());
    }
}