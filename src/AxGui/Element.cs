// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using SkiaSharp;

namespace AxGui
{

    public class Element
    {
        public readonly ElemenetAttributs Attributes = new ElemenetAttributs();
        public readonly ElementStyle Style = new ElementStyle();
        public readonly ElementStyle ResolvedStyle = new ElementStyle();

        public Element? Parent;

        /// <summary>
        /// Logical childs, for example items in a scroll view.
        /// </summary>
        public List<Element> Children;

        /// <summary>
        /// Logical childs and additional childs like the scrollbar of a scroll view.
        /// </summary>
        protected List<Element> ChildrenInternal;
        internal RenderContext _RenderContext = new RenderContext();

        public Element()
        {
            Children = new List<Element>();
            ChildrenInternal = Children;
        }

        /* Order:
         * ComputeStyle
         * ComputeChildBoundsOffers
         * ComputeBounds
         * Render
         */

        protected internal virtual void ComputeStyle(ComputeStyleContext ctx)
        {
            var style = Style;
            var resolved = ResolvedStyle;

            if (style._BoundingChanged)
            {
                resolved._Width = style._Width;
                resolved._Height = style._Height;
                resolved._Margin = style._Margin.Clone();
                resolved._Padding = style._Padding.Clone();
                resolved._BorderWidth = style._BorderWidth.Clone();
                resolved._Anchors = style._Anchors.Clone();
            }

            resolved.Position = style.Position;
            resolved.Display = style.Display;
            resolved.Visibility = style.Visibility;
        }

        internal Box OuterRect;

        internal Box MarginRect;
        internal Box BorderRect;
        internal Box PaddingRect;
        internal Box ClientRect;
        internal Size NaturalSize;

        protected internal virtual void ComputeChildBoundsOffers()
        {
        }

        protected internal virtual void ComputeBounds(ComputeBoundsContext ctx)
        {
            // special case: we're on root
            if (Parent == null)
                OuterRect = ctx.GlobalViewPort;

            Box absAnchors = OuterRect;
            Box relMargin = ResolvedStyle.Margin.ToBox();
            Box relBorder = ResolvedStyle.BorderWidth.ToBox();
            Box relPadding = ResolvedStyle.Padding.ToBox();
            Size relSize = new Size(ResolvedStyle.Width.Number, ResolvedStyle.Height.Number);

            if (ResolvedStyle.Position == StylePosition.Absolute)
            {
                if (ResolvedStyle.Anchors.Top.HasValue() && ResolvedStyle.Anchors.Bottom.HasValue())
                {
                    absAnchors.Top += ResolvedStyle.Anchors.Top.Number;
                    absAnchors.Bottom -= ResolvedStyle.Anchors.Bottom.Number;
                }
                else
                {
                    var diffHeight = relSize.Height + relPadding.TopBottom + relBorder.TopBottom + relMargin.TopBottom;
                    if (ResolvedStyle.Anchors.Top.HasValue())
                    {
                        absAnchors.Top += ResolvedStyle.Anchors.Top.Number;
                        absAnchors.Bottom = absAnchors.Top + diffHeight;
                    }
                    else if (ResolvedStyle.Anchors.Bottom.HasValue())
                    {
                        absAnchors.Bottom -= ResolvedStyle.Anchors.Bottom.Number;
                        absAnchors.Top = absAnchors.Bottom - diffHeight;
                    }
                }
            }

            MarginRect = absAnchors;
            BorderRect = MarginRect.Substract(relMargin);
            PaddingRect = BorderRect.Substract(relBorder);
            ClientRect = PaddingRect.Substract(relPadding);
        }

        private static SKPaint DebugMarginPaint = new SKPaint { Color = new SKColor(174, 129, 82) };
        private static SKPaint DebugBorderPaint = new SKPaint { Color = new SKColor(227, 195, 129) };
        private static SKPaint DebugPaddingPaint = new SKPaint { Color = new SKColor(183, 196, 127) };
        private static SKPaint DebugClientPaint = new SKPaint { Color = new SKColor(135, 178, 188) };

        public virtual void OnRender(RenderContext ctx)
        {
            ctx.Reset();
            ctx.Commands.Add(new DrawActionCommand(x =>
            {
                x.Canvas.DrawRect(MarginRect.ToSKRect(), DebugMarginPaint);
                x.Canvas.DrawRect(BorderRect.ToSKRect(), DebugBorderPaint);
                x.Canvas.DrawRect(PaddingRect.ToSKRect(), DebugPaddingPaint);
                x.Canvas.DrawRect(ClientRect.ToSKRect(), DebugClientPaint);
            }));
        }

    }

}
