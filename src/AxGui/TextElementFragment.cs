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
        //public SKPoint DrawPosition;

        public override void Render(RenderContext ctx)
        {
            ctx.Commands.Add(new DrawActionCommand(x =>
            {
                x.Canvas.DrawRect(ClientRect.Left, ClientRect.Top, ClientRect.Right, ClientRect.Bottom, DebugBorderPaint);
                x.Canvas.DrawText(Text, ClientRect.Left, ClientRect.Top + Paint.TextSize, Paint);
            }));
        }

        public override string ToString()
        {
            return $"Text: {Text}";
        }

    }

}
