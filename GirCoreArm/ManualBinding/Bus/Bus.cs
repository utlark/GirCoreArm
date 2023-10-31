using System;
using GstSharp.ManualBinding.Bus.Internal;

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
        TimedPopFiltered(ulong.MaxValue);
    }

    public void TimedPopFiltered(ulong timeout)
    {
        Methods.TimedPopFiltered(_handle, timeout, MessageType.Eos | MessageType.Error);
    }
}