using System;
using System.Runtime.InteropServices;
using GObject;
using Gst;
using Gst.Internal;
using Bin = Gst.Bin;
using Caps = Gst.Internal.Caps;
using Constants = Gst.Constants;
using Element = Gst.Element;
using ElementFactory = Gst.ElementFactory;
using Functions = Gst.Functions;
using Message = Gst.Internal.Message;
using MiniObject = Gst.Internal.MiniObject;
using Structure = Gst.Internal.Structure;

namespace GstSharp;

public class CustomPipeline
{
    public CustomPipeline(bool debugActive = false, DebugLevel debugLevel = DebugLevel.Warning)
    {
        if (debugActive)
        {
            Functions.DebugSetActive(debugActive);
            Functions.DebugSetDefaultThreshold(debugLevel);
        }

        // ReSharper disable StringLiteralTypo
        RtspSrc = ElementFactory.Make("rtspsrc", null);
        Watchdog = ElementFactory.Make("watchdog", null);
        TypeFind = ElementFactory.Make("typefind", null);
        RtpH264DePay = ElementFactory.Make("rtph264depay", null);
        H264Parse = ElementFactory.Make("h264parse", null);
        CapsFilter = ElementFactory.Make("capsfilter", null);
        AvDecH264 = ElementFactory.Make("avdec_h264", null);
        XvImageSink = ElementFactory.Make("xvimagesink", null);
        Pipeline = Gst.Pipeline.New(null);
        // ReSharper restore StringLiteralTypo

        if (Pipeline == null || RtspSrc == null || Watchdog == null || TypeFind == null || RtpH264DePay == null || H264Parse == null || CapsFilter == null || AvDecH264 == null || XvImageSink == null)
        {
            Program.PrintError("Not all elements could be created");
            throw new NullReferenceException("Not all elements could be created");
        }

        var bin = (Bin)Pipeline;
        bin.Add(RtspSrc);
        bin.Add(Watchdog);
        bin.Add(TypeFind);
        bin.Add(RtpH264DePay);
        bin.Add(H264Parse);
        bin.Add(CapsFilter);
        bin.Add(AvDecH264);
        bin.Add(XvImageSink);

        if (!Watchdog.Link(TypeFind) || !TypeFind.Link(RtpH264DePay) || !RtpH264DePay.Link(H264Parse) || !H264Parse.Link(CapsFilter) || !CapsFilter.Link(AvDecH264) || !AvDecH264.Link(XvImageSink))
        {
            Program.PrintError("Elements could not be linked");
            Pipeline.Unref();
            throw new NullReferenceException("Elements could not be linked");
        }

        //RtspSrc.SetProperty("location", new Value("rtsp://10.59.219.12/h264"));
        RtspSrc.SetProperty("location", new Value("rtsp://admin:admin@10.15.51.120/0"));
        RtspSrc.SetProperty("latency", new Value(0));
        Watchdog.SetProperty("timeout", new Value(10000));

        RtspSrc.OnPadAdded += OnPadAdded;
    }

    private Element? Pipeline { get; }
    private Element? RtspSrc { get; }
    private Element? Watchdog { get; }
    private Element? TypeFind { get; }
    private Element? RtpH264DePay { get; }
    private Element? H264Parse { get; }
    private Element? CapsFilter { get; }
    private Element? AvDecH264 { get; }
    private Element? XvImageSink { get; }

    public void Play()
    {
        var terminate = false;

        var ret = Pipeline!.SetState(State.Playing);
        switch (ret)
        {
            case StateChangeReturn.Failure:
                Program.PrintError("Unable to set the pipeline to the playing state");
                Pipeline.Unref();
                throw new Exception("Unable to set the pipeline to the playing state");
            case StateChangeReturn.Success:
                Console.WriteLine($"Pipeline state change return: '{ret}'");
                break;
            case StateChangeReturn.Async:
                Console.WriteLine($"Pipeline state change return: '{ret}'");
                break;
            case StateChangeReturn.NoPreroll:
                Console.WriteLine($"Pipeline state change return: '{ret}'");
                break;
            default:
                Program.PrintError("Unexpected message received");
                break;
        }

        var bus = Pipeline.GetBus()!;
        do
        {
            var msg = bus.TimedPopFiltered(Constants.CLOCK_TIME_NONE, MessageType.StateChanged | MessageType.Error | MessageType.Eos);

            switch ((MessageType)Marshal.ReadInt64(new IntPtr(msg.Handle.DangerousGetHandle().ToInt64() + 64)))
            {
                case MessageType.Error:
                    Message.ParseError(msg.Handle, out var err, out var debugInfo);

                    var messagePtr = Marshal.ReadInt64(new IntPtr(err.DangerousGetHandle().ToInt64() + 8));
                    var errorMessage = Marshal.PtrToStringUTF8(new IntPtr(messagePtr));

                    var gstObjectPtr = Marshal.ReadInt64(new IntPtr(msg.Handle.DangerousGetHandle().ToInt64() + 80));
                    var namePtr = Marshal.ReadInt64(new IntPtr(gstObjectPtr + 32));
                    var src = Marshal.PtrToStringUTF8(new IntPtr(namePtr));

                    Program.PrintError($"Error received from element '{src}': '{errorMessage}'");
                    Program.PrintError($"Debugging information: '{debugInfo.ConvertToString()}'");
                    GLib.Internal.Functions.ClearError(out err);
                    GLib.Internal.Functions.Free(debugInfo.DangerousGetHandle());
                    terminate = true;
                    break;
                case MessageType.Eos:
                    Console.WriteLine("End-Of-Stream reached");
                    terminate = true;
                    break;
                case MessageType.StateChanged:
                    if (Pipeline.Handle.ToInt64().Equals(Marshal.ReadInt64(new IntPtr(msg.Handle.DangerousGetHandle().ToInt64() + 80))))
                    {
                        Message.ParseStateChanged(msg.Handle, out var oldStatePtr, out var newStatePtr, out _);
                        Console.WriteLine($"Pipeline state changed from '{(State)oldStatePtr.ToInt64()}' to '{(State)newStatePtr.ToInt64()}'");
                    }

                    break;
                case MessageType.Unknown:
                case MessageType.Warning:
                case MessageType.Info:
                case MessageType.Tag:
                case MessageType.Buffering:
                case MessageType.StateDirty:
                case MessageType.StepDone:
                case MessageType.ClockProvide:
                case MessageType.ClockLost:
                case MessageType.NewClock:
                case MessageType.StructureChange:
                case MessageType.StreamStatus:
                case MessageType.Application:
                case MessageType.Element:
                case MessageType.SegmentStart:
                case MessageType.SegmentDone:
                case MessageType.DurationChanged:
                case MessageType.Latency:
                case MessageType.AsyncStart:
                case MessageType.AsyncDone:
                case MessageType.RequestState:
                case MessageType.StepStart:
                case MessageType.Qos:
                case MessageType.Progress:
                case MessageType.Toc:
                case MessageType.ResetTime:
                case MessageType.StreamStart:
                case MessageType.NeedContext:
                case MessageType.HaveContext:
                case MessageType.Extended:
                case MessageType.DeviceAdded:
                case MessageType.DeviceRemoved:
                case MessageType.PropertyNotify:
                case MessageType.StreamCollection:
                case MessageType.StreamsSelected:
                case MessageType.Redirect:
                case MessageType.DeviceChanged:
                case MessageType.InstantRateRequest:
                case MessageType.Any:
                default:
                    Program.PrintError("Unexpected message received");
                    break;
            }

            MiniObject.Unref(new MiniObjectOwnedHandle(msg.Handle.DangerousGetHandle()));
        } while (!terminate);

        bus.Unref();
        Pipeline.SetState(State.Null);
        Pipeline.Unref();
    }

    private void OnPadAdded(Element sender, Element.PadAddedSignalArgs signalArgs)
    {
        var sinkPad = Watchdog?.GetStaticPad("sink");
        if (sinkPad == null)
            throw new NullReferenceException("Unable to get the sink pad from 'Watchdog' element");

        var newPad = signalArgs.NewPad;
        Program.PrintColored($"Received new pad '{newPad.Name}' from '{sender.Name}'", ConsoleColor.Yellow);

        if (sinkPad.IsLinked())
        {
            Program.PrintColored("We are already linked. Ignoring", ConsoleColor.Yellow);
            sinkPad.Unref();
            return;
        }

        var newPadCaps = newPad.GetCurrentCaps();
        var newPadStruct = Caps.GetStructure(newPadCaps.Handle, 0);
        var newPadType = Structure.GetName(newPadStruct);
        var newPadTypeString = newPadType.ConvertToString();
        if (!newPadTypeString.Equals("application/x-rtp"))
        {
            Program.PrintColored($"It has type '{newPadTypeString}' which is not rtp video. Ignoring", ConsoleColor.Yellow);
            MiniObject.Unref(new MiniObjectOwnedHandle(newPadCaps.Handle.DangerousGetHandle()));
            sinkPad.Unref();
            return;
        }

        var ret = newPad.Link(sinkPad);
        if (ret < PadLinkReturn.Ok)
        {
            Program.PrintError($"Type is '{newPadTypeString}' but link failed");
        }
        else
        {
            Program.PrintColored($"Link succeeded (type '{newPadTypeString}')", ConsoleColor.Yellow);
            Watchdog?.SetProperty("timeout", new Value(1000));
        }

        MiniObject.Unref(new MiniObjectOwnedHandle(newPadCaps.Handle.DangerousGetHandle()));
        sinkPad.Unref();
    }
}