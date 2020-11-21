// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
            Layouter.ViewPort = new Box(0, 0, CurrentSize.X, CurrentSize.Y);

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

            Watcher = new FileSystemWatcher("../../..");
            Watcher.Changed += (s, e) => BuildUI();
            Watcher.EnableRaisingEvents = true;

            BuildUI();
        }

        private SKPaint Paint;
        private Stopwatch FPSCounter;
        private LayoutProcessor Layouter;
        private CommandRecorder Recorder;
        private CommandExecutor Executor;
        private Element el;

        FileSystemWatcher Watcher;

        private void BuildUI()
        {
            var doc = Document.FromFile("../../../Test.html");

            var styles = StyleCollection.FromFile("../../../Theme.css");

            el = doc.Body;

            doc.GetElementById("dialog").ScrollOffset.Y = 10;

            //var styles = doc.Styles;

            //el = new Element();
            //el.Data = "root";

            //el.Style.Display = StyleDisplay.Block;
            //el.Style.Position = StylePosition.Absolute;
            //el.CssClass = "root";

            Layouter = new LayoutProcessor();
            Layouter.ViewPort = new Box(0, 0, CurrentSize.X, CurrentSize.Y);
            Layouter.Styles = styles;

            Recorder = new CommandRecorder();
            Executor = new CommandExecutor();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Viewport(0, 0, (int)CurrentSize.X, (int)CurrentSize.Y);
            GL.ClearColor(Color4.Beige);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            using (new SKAutoCanvasRestore(surface.Canvas, true))
            {
                // We've modified opengl state, so let's reset
                grContext.ResetContext();

                var canvas = surface.Canvas;

                Layouter.Process(el);
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
