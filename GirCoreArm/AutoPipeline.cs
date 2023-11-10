using System;
using GObject;
using Gst;
using Caps = Gst.Internal.Caps;
using Functions = Gst.Functions;
using Structure = Gst.Internal.Structure;

namespace GstSharp;

public class AutoPipeline
{
    public AutoPipeline(bool debugActive = false, DebugLevel debugLevel = DebugLevel.Warning)
    {
        if (debugActive)
        {
            Functions.DebugSetActive(debugActive);
            Functions.DebugSetDefaultThreshold(debugLevel);
        }

        // ReSharper disable StringLiteralTypo
        RtspSrc = ElementFactory.Make("rtspsrc", null);
        Watchdog = ElementFactory.Make("watchdog", null);
        RtpH264DePay = ElementFactory.Make("rtph264depay", null);
        H264Parse = ElementFactory.Make("h264parse", null);
        DecodeBin = ElementFactory.Make("decodebin", null);
        AutoVideoSink = ElementFactory.Make("autovideosink", null);
        Pipeline = Gst.Pipeline.New(null);
        // ReSharper restore StringLiteralTypo

        if (Pipeline == null || RtspSrc == null || Watchdog == null || RtpH264DePay == null || H264Parse == null || DecodeBin == null || AutoVideoSink == null)
        {
            Program.PrintError("Not all elements could be created");
            throw new NullReferenceException("Not all elements could be created");
        }

        var bin = (Bin)Pipeline;
        bin.Add(RtspSrc);
        bin.Add(Watchdog);
        bin.Add(RtpH264DePay);
        bin.Add(H264Parse);
        bin.Add(DecodeBin);
        bin.Add(AutoVideoSink);

        if (!Watchdog.Link(RtpH264DePay) || !RtpH264DePay.Link(H264Parse) || !H264Parse.Link(DecodeBin))
        {
            Program.PrintError("Elements could not be linked");
            Pipeline.Unref();
            throw new NullReferenceException("Elements could not be linked");
        }

        //RtspSrc.SetProperty("location", new Value("rtsp://10.59.219.12/h264"));
        RtspSrc.SetProperty("location", new Value("rtsp://admin:admin@10.15.51.120/0"));
        RtspSrc.SetProperty("latency", new Value(300));
        Watchdog.SetProperty("timeout", new Value(10000));

        RtspSrc.OnPadAdded += OnPadAdded;
        DecodeBin.OnPadAdded += OnPadAdded;
    }

    private Element? Pipeline { get; }
    private Element? RtspSrc { get; }
    private Element? Watchdog { get; }
    private Element? RtpH264DePay { get; }
    private Element? H264Parse { get; }
    private Element? DecodeBin { get; }
    private Element? AutoVideoSink { get; }

    public void Play()
    {
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

        Pipeline.GetBus()!.WaitForEndOrError();
        Pipeline.SetState(State.Null);
    }

    private void OnPadAdded(Element sender, Element.PadAddedSignalArgs signalArgs)
    {
        Pad? sinkPad;
        if (sender.Name != null && sender.Name.Contains("rtspsrc"))
            sinkPad = Watchdog?.GetStaticPad("sink");
        else if (sender.Name != null && sender.Name.Contains("decodebin"))
            sinkPad = AutoVideoSink?.GetStaticPad("sink");
        else
            throw new NullReferenceException($"Unable to get the sink pad from element next to '{sender.Name}'");

        if (sinkPad == null)
            throw new NullReferenceException($"Unable to get the sink pad from element next to '{sender.Name}'");

        var newPad = signalArgs.NewPad;
        Program.PrintColored($"Received new pad '{newPad.Name}' from '{sender.Name}'", ConsoleColor.Yellow);

        if (sinkPad.IsLinked())
        {
            Program.PrintColored("We are already linked. Ignoring", ConsoleColor.Yellow);
            return;
        }

        var newPadCaps = newPad.GetCurrentCaps();
        var newPadStruct = Caps.GetStructure(newPadCaps.Handle, 0);
        var newPadType = Structure.GetName(newPadStruct);
        var newPadTypeString = newPadType.ConvertToString();
        if (!newPadTypeString.Equals("application/x-rtp") && !newPadTypeString.Equals("video/x-raw"))
        {
            Program.PrintColored($"It has type '{newPadTypeString}' which is not rtp video or raw. Ignoring", ConsoleColor.Yellow);
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
    }
}