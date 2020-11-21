// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using SkiaSharp;

namespace AxGui
{

    internal class TextElementFragment : Element
    {

        public TextElementFragment(string value, SKPaint paint, SKFont font)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException($"{nameof(value)} cannot be empty");

            Paint = paint;
            Text = value;
            Font = font;
            TextBlob = SKTextBlob.Create(Text, Font);
        }

        internal readonly SKPaint Paint;
        internal readonly string Text;
        private SKTextBlob TextBlob;
        public SKPoint DrawPosition;
        private readonly SKFont Font;

        protected override void ResolveStyle(ProcessLayoutContext ctx)
        {
            // do not call the base
        }

        public override void Render(RenderContext ctx)
        {
            RenderBorderAndBackground(ctx);
            var clientRectInner = ClientRect.Substract(1);

            ctx.Commands.Add(new DrawActionCommand(x =>
            {
                x.Canvas.DrawText(TextBlob, ClientRect.Left + DrawPosition.X, ClientRect.Top + DrawPosition.Y, Paint);
            }));
        }

        public override string ToString()
        {
            return $"Text: {Text}";
        }

    }

}
