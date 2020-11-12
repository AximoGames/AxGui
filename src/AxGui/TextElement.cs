// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

namespace AxGui
{

    public class TextElement : Element
    {
        public string? Content;
        private readonly SKPaint Paint = new SKPaint();
        public float TextSize { get => Paint.TextSize; set => Paint.TextSize = value; }

        private List<TextElementFragment> TmpFragments = new List<TextElementFragment>();

        public override void Render(RenderContext ctx)
        {
            RenderBorderAndBackground(ctx);

            if (Content != null)
            {
                Paint.Color = new SKColor(0, 0, 200);

                TmpFragments.Clear();

                var span = Content.AsSpan();

                float posY = 0;

                while (span.Length > 0)
                {
                    var brIdx = -1;
                    var spIdx = -1;

                    var num = (int)Paint.BreakText(span, ClientRect.Width);

                    // detect space
                    if (num < span.Length)
                    {
                        spIdx = span.LastIndexOf(' ');
                        if (spIdx > -1)
                            num = spIdx;

                        // TODO: Decrease num, if there are more spaces: 'text    ' --> 'text'
                    }

                    var f = new TextElementFragment(new string(span.Slice(0, num)));
                    f.DrawPosition = new SKPoint(ClientRect.Left, ClientRect.Top + posY + Paint.TextSize);
                    TmpFragments.Add(f);

                    posY += Paint.TextSize;
                    if (span.Length == num)
                        break;
                    span = span.Slice(num + 1);

                    //break;
                }

                var fragments = TmpFragments.ToList();

                ctx.Commands.Add(new DrawActionCommand(x =>
                {
                    var len = fragments.Count;
                    for (var i = 0; i < len; i++)
                    {
                        var frag = fragments[i];
                        x.Canvas.DrawText(frag.Text, frag.DrawPosition, Paint);
                    }

                    //x.Canvas.DrawText(Content, ClientRect.Left, ClientRect.Bottom, Paint);
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
