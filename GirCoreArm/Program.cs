using System;
using Gst;

namespace GstSharp;

internal static class Program
{
    private const string HelpMessage = "Usage: GstSharp <mode> <pipeline>\n" +
                                       "\n" +
                                       "Mode:\n" +
                                       "    core     c   - use GirCore.Gst-1.0\n" +
                                       "    manual   m   - use manual binding";

    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine(HelpMessage);
            return;
        }

        var pipelineString = args.Length > 1 && !string.IsNullOrEmpty(args[1]) ? args[1] : "playbin uri=playbin uri=http://ftp.halifax.rwth-aachen.de/blender/demo/movies/ToS/tears_of_steel_720p.mov";
        Console.WriteLine($"Playing: {pipelineString}");

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
}