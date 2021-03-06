﻿// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime;
using OpenToolkit;
using OpenToolkit.Windowing.Desktop;

namespace AxGui.Sample.OpenGL
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
            var app = new SampleApplication(GameWindowSettings.Default, NativeWindowSettings.Default);
            app.Run();
        }

    }
}
