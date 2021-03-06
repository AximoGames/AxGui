﻿// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.Extensions.FileProviders;
using SkiaSharp;

namespace AxGui
{

    public class TextElement : Element
    {
        public string? Content;
        private readonly SKPaint Paint = new SKPaint();
        public float TextSize { get => Paint.TextSize; set => Paint.TextSize = value; }
        public float TextHeight => Paint.FontMetrics.Descent - Paint.FontMetrics.Ascent;

        public TextElement()
        {
            Paint = new SKPaint(new SKFont(FontManager.DefaultTypeFace));
            PassThrough = true;
        }

        protected internal override void ComputeBoundsSelf(ProcessLayoutContext ctx)
        {
            base.ComputeBoundsSelf(ctx);

            Children.Clear();

            var span = Content.AsSpan();

            float posY = 0;

            var parent = GetParentBlockElement(ctx);
            var availableWidth = parent!.ClientRect.Width;

            bool first = true;
            while (span.Length > 0)
            {
                var brIdx = -1;
                var spIdx = -1;

                float measuredWidth;
                var num = (int)Paint.BreakText(span, first ? availableWidth - parent.ProcessLayoutContext.RowPosition.X : availableWidth, out measuredWidth);

                // detect space
                if (num < span.Length)
                {
                    if (ResolvedStyle.Display == StyleDisplay.InlineBlock)
                    {
                        num = span.Length;
                    }
                    else
                    {

                        spIdx = span.Slice(0, num).LastIndexOf(' ');
                        if (spIdx > -1)
                            num = spIdx;

                        // TODO: Decrease num, if there are more spaces: 'text    ' --> 'text'
                    }
                }

                var texHeight = TextHeight;

                var finalText = new string(span.Slice(0, num));
                var f = new TextElementFragment(finalText, Paint);
                ResolvedStyle.CopyTo(f.ResolvedStyle);
                measuredWidth = Paint.MeasureText(finalText);
                //f.DrawPosition = new SKPoint(ClientRect.Left, ClientRect.Top + posY + Paint.TextSize);
                f.ResolvedStyle.Width = measuredWidth;

                if (f.ResolvedStyle.Height.Unit == StyleUnit.Unset)
                    f.ResolvedStyle.Height = TextHeight;

                f.ResolvedStyle.Display = StyleDisplay.InlineBlock;
                f.ResolvedStyle.Position = StylePosition.Static;

                var h = f.ResolvedStyle.Height.Number;
                f.DrawPosition.Y = TextSize + ((h - TextHeight) / 2);

                AddChild(f);

                posY += Paint.TextSize;
                if (span.Length == num)
                    break;
                span = span.Slice(spIdx == -1 ? num : num + 1);

                first = false;
                //break;
            }
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
