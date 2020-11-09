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
        private const SKColorType colorType = SKColorType.Rgba8888;
        private const GRSurfaceOrigin surfaceOrigin = GRSurfaceOrigin.BottomLeft;

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
            var maxSamples = grContext.GetMaxSurfaceSampleCount(colorType);
            if (samples > maxSamples)
                samples = maxSamples;
            var glInfo = new GRGlFramebufferInfo((uint)framebuffer, colorType.ToGlSizedFormat());
            renderTarget = new GRBackendRenderTarget(CurrentSize.X, CurrentSize.Y, 0, 0, glInfo);

            // create the surface
            surface?.Dispose();
            surface = SKSurface.Create(grContext, renderTarget, surfaceOrigin, colorType);

            paint = new SKPaint { Color = new SKColor(0, 128, 0), IsAntialias = true };
        }

        protected override void OnLoad()
        {
            CurrentSize = ClientSize;

            InitSkia();
        }

        SKPaint paint;

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
                canvas.DrawText("Hello", 0, 0, paint);

                var el = new Element();
                el.Style.Position = StylePosition.Absolute;

                el.Style.MinHeight = 50;
                el.Style.Height = "50%";
                //el.Style.Anchors.Top = 10;

                el.Style.Anchors = new BoxModelRect
                {
                    //Top = 5,
                    Bottom = 5,
                };

                el.Style.Margin = 5;
                el.Style.BorderWidth = 5;
                el.Style.Padding = 5;

                //el.Style.Anchors.Bottom = 20;

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
