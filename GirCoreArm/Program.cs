using System;
using Gst;

var pipelineString = args.Length >= 1 && !string.IsNullOrEmpty(args[0]) ? args[0] : "playbin uri=playbin uri=file:///opt/Costa_Rica_HD_25Fps.avi";

Module.Initialize();
Console.WriteLine($"Playing: {pipelineString}");

Application.Init();
var pipeline = Functions.ParseLaunch(pipelineString);
pipeline.SetState(State.Playing);
pipeline.GetBus()!.WaitForEndOrError();
pipeline.SetState(State.Null);