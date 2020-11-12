// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using SkiaSharp;

namespace AxGui
{

    public class TextElement : Element
    {
        public string? Content;
        private readonly SKPaint Paint = new SKPaint();
        public float TextSize { get => Paint.TextSize; set => Paint.TextSize = value; }

        public override void Render(RenderContext ctx)
        {
            RenderBorderAndBackground(ctx);

            if (Content != null)
            {
                Paint.Color = new SKColor(0, 0, 200);

                ctx.Commands.Add(new DrawActionCommand(x =>
                {
                    x.Canvas.DrawText(Content, ClientRect.Left, ClientRect.Bottom, Paint);
                }));
            }

            RenderChildren(ctx);
        }

        protected override void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            Paint.Dispose();

            base.Dispose(disposing);
        }

    }

}
