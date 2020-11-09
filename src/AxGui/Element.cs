// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using SkiaSharp;

namespace AxGui
{

    public enum Axis
    {
        X,
        Y,
    }

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

        protected internal virtual void ComputeStyle(ProcessLayoutContext ctx)
        {
            var style = Style;
            var resolved = ResolvedStyle;

            if (style._BoundingChanged)
            {
                resolved._Size = style._Size.Normalize(ctx);
                resolved._MinSize = style._MinSize.Normalize(ctx);
                resolved._MaxSize = style._MaxSize.Normalize(ctx);
                resolved._Margin = style._Margin.Normalize(ctx);
                resolved._Padding = style._Padding.Normalize(ctx);
                resolved._BorderWidth = style._BorderWidth.Normalize(ctx);
                resolved._Anchors = style._Anchors.Normalize(ctx);
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

        protected internal virtual void ComputeBounds(ProcessLayoutContext ctx)
        {
            // special case: we're on root
            if (Parent == null)
                OuterRect = ctx.LocalViewPort;

            Box absAnchors = OuterRect;
            var absCenter = absAnchors.Center;

            Box relMargin = ResolvedStyle._Margin.ToBox();
            Box relBorder = ResolvedStyle._BorderWidth.ToBox();
            Box relPadding = ResolvedStyle._Padding.ToBox();

            Size relSize = ResolvedStyle._Size.ToSize();
            Size relMinSize = ResolvedStyle._MinSize.ToSize();
            Size relMaxSize = ResolvedStyle._MaxSize.ToSize();

            var decorationSize = new Size(
                    relMargin.LeftRight + relBorder.LeftRight + relPadding.LeftRight,
                    relMargin.TopBottom + relBorder.TopBottom + relPadding.TopBottom);

            // top | left
            bool normalDirection = true;

            if (ResolvedStyle.Position == StylePosition.Absolute)
            {
                if (ResolvedStyle._Anchors.Top.HasValue() && ResolvedStyle._Anchors.Bottom.HasValue())
                {
                    absAnchors.Top += ResolvedStyle._Anchors.Top.Number;
                    absAnchors.Bottom -= ResolvedStyle._Anchors.Bottom.Number;
                }
                else
                {
                    var diffHeight = relSize.Height + decorationSize.Height;
                    if (ResolvedStyle._Margin.Top.Unit == StyleUnit.Auto
                        && ResolvedStyle._Margin.Bottom.Unit == StyleUnit.Auto)
                    {
                        var size = diffHeight;
                        var halfSize = size / 2;

                        absAnchors.Top = absCenter.Y - halfSize;
                        absAnchors.Bottom = absCenter.Y + halfSize;
                    }
                    else
                    {
                        if (ResolvedStyle._Anchors.Top.HasValue())
                        {
                            absAnchors.Top += ResolvedStyle._Anchors.Top.Number;
                            absAnchors.Bottom = absAnchors.Top + diffHeight;
                        }
                        else if (ResolvedStyle._Anchors.Bottom.HasValue())
                        {
                            normalDirection = false;
                            absAnchors.Bottom -= ResolvedStyle._Anchors.Bottom.Number;
                            absAnchors.Top = absAnchors.Bottom - diffHeight;
                        }
                    }
                }
            }

            MarginRect = absAnchors;
            BorderRect = MarginRect.Substract(relMargin);
            PaddingRect = BorderRect.Substract(relBorder);
            ClientRect = PaddingRect.Substract(relPadding);

            if (ResolvedStyle.MaxHeight.HasValue() && ClientRect.Height > relMaxSize.Height)
            {
                var diff = ClientRect.Height - relMaxSize.Height;
                if (normalDirection)
                {
                    ClientRect.Bottom -= diff;
                    PaddingRect.Bottom -= diff;
                    BorderRect.Bottom -= diff;
                    MarginRect.Bottom -= diff;
                }
                else
                {
                    ClientRect.Top += diff;
                    PaddingRect.Top += diff;
                    BorderRect.Top += diff;
                    MarginRect.Top += diff;
                }
            }

            if (ResolvedStyle.MinHeight.HasValue() && ClientRect.Height < relMinSize.Height)
            {
                var diff = relMinSize.Height - ClientRect.Height;
                if (normalDirection)
                {
                    ClientRect.Bottom += diff;
                    PaddingRect.Bottom += diff;
                    BorderRect.Bottom += diff;
                    MarginRect.Bottom += diff;
                }
                else
                {
                    ClientRect.Top -= diff;
                    PaddingRect.Top -= diff;
                    BorderRect.Top -= diff;
                    MarginRect.Top -= diff;
                }
            }
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
