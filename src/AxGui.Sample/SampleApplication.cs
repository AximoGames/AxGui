// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Desktop;
using SkiaSharp;

namespace AxGui.Sample.OpenGL
{
    public class SampleApplication : GameWindow
    {

        public SampleApplication(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        private GRContext grContext;
        private GRBackendRenderTarget renderTarget;
        private SKSurface surface;
        private const SKColorType ColorType = SKColorType.Rgba8888;
        private const GRSurfaceOrigin SurfaceOrigin = GRSurfaceOrigin.BottomLeft;

        protected override void OnResize(ResizeEventArgs e)
        {
            if (CurrentSize == e.Size)
                return;

            CurrentSize = e.Size;

            InitSkia();
        }

        private Vector2i CurrentSize;

        private void InitSkia()
        {
            if (grContext == null)
            {
                var glInterface = GRGlInterface.Create();
                grContext = GRContext.CreateGl(glInterface);
            }

            // define the surface properties
            // create or update the dimensions
            renderTarget?.Dispose();
            GL.GetInteger(GetPName.FramebufferBinding, out var framebuffer);
            var stencil = 0;
            GL.GetInteger(GetPName.Samples, out var samples);
            var maxSamples = grContext.GetMaxSurfaceSampleCount(ColorType);
            if (samples > maxSamples)
                samples = maxSamples;
            var glInfo = new GRGlFramebufferInfo((uint)framebuffer, ColorType.ToGlSizedFormat());
            renderTarget = new GRBackendRenderTarget(CurrentSize.X, CurrentSize.Y, 0, 0, glInfo);

            // create the surface
            surface?.Dispose();
            surface = SKSurface.Create(grContext, renderTarget, SurfaceOrigin, ColorType);

            Paint = new SKPaint { Color = new SKColor(0, 128, 0), IsAntialias = true };
        }

        protected override void OnLoad()
        {
            CurrentSize = ClientSize;
            //VSync = VSyncMode.On;

            InitSkia();
            FPSCounter = new Stopwatch();
            FPSCounter.Start();

            BuildUI();
        }

        private SKPaint Paint;
        private Stopwatch FPSCounter;
        private CommandRecorder Recorder;
        private CommandExecutor Executor;
        private Element el;

        private void BuildUI()
        {
            el = new Element();
            el.Data = "root";

            el.Style.Display = StyleDisplay.Block;
            el.Style.Position = StylePosition.Absolute;
            el.Style.Height = 320;
            el.Style.Width = 220;
            el.Style.BorderWidth = 5;
            el.Style.Padding = 5;

            Element child;
            Element box;

            var box1 = box = new Element();
            box.Data = "b1";
            box.Style.Display = StyleDisplay.Block;
            box.Style.Position = StylePosition.Static;
            box.Style.Width = 20;
            box.Style.Height = 20;
            el.AddChild(box);

            var child1 = child = new TextElement();
            child.Data = "child";
            child.Style.Width = 5;
            child.Style.Height = 30;
            child.Style.Display = StyleDisplay.Inline;
            child.Style.Position = StylePosition.Static;
            (child as TextElement).Content = "Testduck Testduck2 Testduck3";
            (child as TextElement).TextSize = 20;
            el.AddChild(child);

            var box2 = box = new Element();
            box.Data = "b1";
            box.Style.Display = StyleDisplay.InlineBlock;
            box.Style.Position = StylePosition.Static;
            box.Style.Width = 20;
            box.Style.Height = 20;
            el.AddChild(box);

            var child2 = child = new TextElement();
            child.Data = "child2";
            child.Style.Width = 5;
            child.Style.Height = 30;
            child.Style.Display = StyleDisplay.Inline;
            child.Style.Position = StylePosition.Static;
            (child as TextElement).Content = "Testduck4 Testduck5 Testduck6";
            (child as TextElement).TextSize = 20;
            el.AddChild(child);

            var box3 = box = new Element();
            box.Data = "b3";
            box.Style.Display = StyleDisplay.Block;
            box.Style.Position = StylePosition.Static;
            box.Style.Width = 20;
            box.Style.Height = 20;
            el.AddChild(box);

            var child3 = child = new TextElement();
            child.Data = "child3";
            child.Style.Height = 30;
            child.Style.Width = 5;
            child.Style.Display = StyleDisplay.Inline;
            child.Style.Position = StylePosition.Static;
            (child as TextElement).Content = "Testduck4 Testduck5 Test|ö|└|²³|ₚ|☑|😂";
            (child as TextElement).TextSize = 20;
            el.AddChild(child);

            var layouter = new LayoutProcessor();
            layouter.ViewPort = new Box(0, 0, ClientSize.X, ClientSize.Y);
            //layouter.ViewPort = new Box(0, 0, ClientSize.X, 20);
            layouter.Process(el);

            Recorder = new CommandRecorder();
            Executor = new CommandExecutor();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            GL.ClearColor(Color4.Beige);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            using (new SKAutoCanvasRestore(surface.Canvas, true))
            {
                // We've modified opengl state, so let's reset
                grContext.ResetContext();

                var canvas = surface.Canvas;

                Recorder.Record(el);
                Executor.Execute(Recorder, canvas);

                canvas.Flush();
            }

            //System.Threading.Thread.Sleep(500);

            SwapBuffers();

            var ticks = FPSCounter.ElapsedTicks;
            var ms = ticks / 10000.0;
            var newFPS = 1000f / (float)ms;
            FPSCounter.Restart();
            CurrentFPS = Smooth(CurrentFPS, newFPS, 0.01f);
            if ((DateTime.UtcNow - LastUpdatedFPS).TotalSeconds > 1)
            {
                LastUpdatedFPS = DateTime.UtcNow;
                Title = $"FPS: {CurrentFPS.ToString("F0", CultureInfo.CurrentCulture)}";
            }
        }

        private float CurrentFPS;
        private DateTime LastUpdatedFPS;

        /// <summary>
        /// Smoothes a value
        /// </summary>
        /// <param name="oldValue">old value</param>
        /// <param name="newValue">new value</param>
        /// <param name="smoothing">Number between 0-1</param>
        /// <returns>the smoothed value</returns>
        private static float Smooth(float oldValue, float newValue, float smoothing)
        {
            if (float.IsInfinity(oldValue))
                return newValue;
            return (newValue * smoothing) + (oldValue * (1.0f - smoothing));
        }
    }

}
