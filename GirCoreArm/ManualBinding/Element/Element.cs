using System;
using GstSharp.ManualBinding.Element.Internal;

namespace GstSharp.ManualBinding.Element;

public class Element
{
    private readonly IntPtr _handle;

    public Element(IntPtr handle)
    {
        _handle = handle;
    }

    public StateChangeReturn SetState(State state)
    {
        return Methods.SetState(_handle, state);
    }

    public Bus.Bus GetBus()
    {
        return new Bus.Bus(Methods.GetBus(_handle));
    }
}