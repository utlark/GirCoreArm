using System;
using Gst;

namespace GstSharp;

internal static class Program
{
    private const string HelpMessage = "Usage: GstSharp <mode> <pipeline>\n" +
                                       "\n" +
                                       "Mode:\n" +
                                       "    core     c   - use GirCore.Gst-1.0\n" +
                                       "    custom   cp  - use GirCore.Gst-1.0 CustomPipeline\n" +
                                       "    arm          - use GirCore.Gst-1.0 CustomPipeline for ARM\n" +
                                       "    manual   m   - use manual binding";

    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine(HelpMessage);
            return;
        }

        // ReSharper disable StringLiteralTypo
        var pipelineString = args.Length > 1 && !string.IsNullOrEmpty(args[1]) ? args[1] : "rtspsrc location=rtsp://admin:admin@10.15.51.120/0 latency=0 ! watchdog ! rtph264depay ! h264parse ! decodebin ! autovideosink";
        if (!args[0].Equals("custom") && !args[0].Equals("cp") && !args[0].Equals("auto") && !args[0].Equals("arm"))
            Console.WriteLine($"Playing: {pipelineString}");
        // ReSharper restore StringLiteralTypo

        switch (args[0])
        {
            case "core":
            case "c":
                Module.Initialize();
                Application.Init();
                var pipeline = Functions.ParseLaunch(pipelineString);
                Console.WriteLine($"SetState Playing: {pipeline.SetState(State.Playing).ToString()}");
                pipeline.GetBus()!.WaitForEndOrError();
                pipeline.SetState(State.Null);
                break;
            case "custom":
            case "cp":
                Module.Initialize();
                Application.Init();
                var customPipeline = new CustomPipeline();
                customPipeline.Play();
                break;
            case "auto":
                Module.Initialize();
                Application.Init();
                new AutoPipeline(true, DebugLevel.Fixme).Play();
                break;
            case "arm":
                Module.Initialize();
                Application.Init();
                new ArmPipeline().Play();
                break;
            case "manual":
            case "m":
                ManualBinding.Module.Initialize();
                ManualBinding.Application.Init();
                var pipelineM = ManualBinding.Gst.Functions.ParseLaunch(pipelineString);
                Console.WriteLine($"SetState Playing: {pipelineM.SetState(ManualBinding.State.Playing).ToString()}");
                pipelineM.GetBus().WaitForEndOrError();
                pipelineM.SetState(ManualBinding.State.Null);
                break;
            default:
                Console.WriteLine(HelpMessage);
                return;
        }
    }

    public static void PrintError(string message)
    {
        PrintColored(message, ConsoleColor.Red);
    }

    public static void PrintColored(string message, ConsoleColor color)
    {
        var temp = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = temp;
    }
}