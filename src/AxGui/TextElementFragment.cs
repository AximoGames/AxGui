// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using SkiaSharp;

namespace AxGui
{

    internal class TextElementFragment : Element
    {

        public TextElementFragment(string value, SKPaint paint)
        {
            Paint = paint;
            Text = value;
        }

        internal readonly SKPaint Paint;
        internal readonly string Text;
        public SKPoint DrawPosition;

        public override void Render(RenderContext ctx)
        {
            RenderBorderAndBackground(ctx);

            var clientRectInner = ClientRect.Substract(1);
            //var textY = (((-Paint.FontMetrics.Ascent + Paint.FontMetrics.Descent) / 2) - Paint.FontMetrics.Descent);

            //var fontHeight = Paint.FontMetrics.Descent - Paint.FontMetrics.Ascent;

            ctx.Commands.Add(new DrawActionCommand(x =>
            {
                x.Canvas.DrawText(Text, ClientRect.Left + DrawPosition.X, ClientRect.Top + DrawPosition.Y, Paint);
            }));
        }

        public override string ToString()
        {
            return $"Text: {Text}";
        }

    }

}
