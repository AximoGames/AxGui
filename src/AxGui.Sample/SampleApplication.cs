// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
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

            InitSkia();
        }

        private SKPaint Paint;

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
                canvas.Clear(new SKColor(128, 0, 128));
                //canvas.Translate(20, 20);
                //Paint.Color = new SKColor(0, 0, 200);
                //Paint.TextSize = 64;
                //canvas.DrawText("Hello", 0, 64, Paint);
                //canvas.DrawRect(0, 0, 100, 100, Paint);

                var el = new Element();
                el.Data = "root";

                el.Style.Position = StylePosition.Absolute;

                el.Style.MinHeight = 55;
                el.Style.MinWidth = 80;

                el.Style.Height = 320;
                el.Style.Width = 220;

                //el.Style.Margin = "auto";
                el.Style.BorderWidth = 5;
                el.Style.Padding = 5;
                //el.Style.Visibility = StyleVisibility.Hidden;

                Element child;

                var child1 = child = new TextElement();
                child.Data = "child";
                child.Style.BorderWidth = 5;
                //child.Style.Width = 55;
                //child.Style.Height = 100;
                //child.Style.Position = StylePosition.Absolute;
                child.Style.Position = StylePosition.Relative;
                child.Style.Display = StyleDisplay.Inline;
                (child as TextElement).Content = "Testganz Testganz2 Testganz3";
                (child as TextElement).TextSize = 20;
                el.AddChild(child);

                var child2 = child = new TextElement();
                child.Data = "child";
                child.Style.BorderWidth = 5;
                //child.Style.Width = 55;
                //child.Style.Height = 100;
                //child.Style.Position = StylePosition.Absolute;
                child.Style.Position = StylePosition.Relative;
                child.Style.Display = StyleDisplay.Inline;
                (child as TextElement).Content = "Testganz4 Testganz5 Testganz6";
                (child as TextElement).TextSize = 20;
                el.AddChild(child);

                //child = new Element();
                //child.Data = "child2";
                //child.Style.BorderWidth = 5;
                //child.Style.Width = 200;
                //child.Style.Height = 30;
                ////child.Style.Position = StylePosition.Absolute;
                //child.Style.Position = StylePosition.Relative;
                //child.Style.Display = StyleDisplay.InlineBlock;
                //el.AddChild(child);

                //child = new Element();
                //child.Data = "child3";
                //child.Style.BorderWidth = 5;
                //child.Style.Width = 200;
                //child.Style.Height = 30;
                ////child.Style.Position = StylePosition.Absolute;
                //child.Style.Position = StylePosition.Relative;
                //child.Style.Display = StyleDisplay.InlineBlock;
                //el.AddChild(child);

                var layouter = new LayoutProcessor();
                layouter.ViewPort = new Box(0, 0, ClientSize.X, ClientSize.Y);
                //layouter.ViewPort = new Box(0, 0, ClientSize.X, 20);
                layouter.Process(el);

                var recorder = new CommandRecorder();
                recorder.Record(el);

                var executor = new CommandExecutor();
                executor.Execute(recorder, canvas);

                canvas.Flush();
            }

            SwapBuffers();
        }

    }

}
