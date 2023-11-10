using System;
using GstSharp.ManualBinding.Bus.Internal;
using JetBrains.Annotations;

namespace GstSharp.ManualBinding.Bus;

public class Bus
{
    private readonly IntPtr _handle;

    public Bus(IntPtr handle)
    {
        _handle = handle;
    }

    public void WaitForEndOrError()
    {
        TimedPopFiltered(ulong.MaxValue, MessageType.Eos | MessageType.Error);
    }

    [UsedImplicitly]
    public IntPtr TimedPopFiltered(ulong timeout, MessageType types)
    {
        return Methods.TimedPopFiltered(_handle, timeout, types);
    }

    public IntPtr PopFiltered(MessageType types)
    {
        return Methods.PopFiltered(_handle, types);
    }
}