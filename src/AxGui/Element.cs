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

            // top | left
            Span<bool> normalDirection = stackalloc bool[2];

            // Debug
            const int axisStart = 0;
            const int axisCount = 2;

            const int Top = 0;
            const int Bottom = 2;
            //const int Width = 0;
            //const int Height = 1;

            var decorationSize = new Size(
                relMargin.LeftRight + relBorder.LeftRight + relPadding.LeftRight,
                relMargin.TopBottom + relBorder.TopBottom + relPadding.TopBottom);

            for (var a = axisStart; a < axisCount; a++)
            {
                normalDirection[a] = true;
                var ax = (Axis)a;

                if (ResolvedStyle.Position == StylePosition.Absolute)
                {
                    if (ResolvedStyle._Anchors[Top + a].HasValue() && ResolvedStyle._Anchors[Bottom + a].HasValue())
                    {
                        absAnchors[Top + a] += ResolvedStyle._Anchors[Top + a].Number;
                        absAnchors[Bottom + a] -= ResolvedStyle._Anchors[Bottom + a].Number;
                    }
                    else
                    {
                        var diffHeight = relSize[a] + decorationSize[a];
                        if (ResolvedStyle._Margin[Top + a].Unit == StyleUnit.Auto
                            && ResolvedStyle._Margin[Bottom + a].Unit == StyleUnit.Auto)
                        {
                            var size = diffHeight;
                            var halfSize = size / 2;

                            absAnchors[Top + a] = absCenter[a] - halfSize;
                            absAnchors[Bottom + a] = absCenter[a] + halfSize;
                        }
                        else
                        {
                            if (ResolvedStyle._Anchors[Top + a].HasValue())
                            {
                                absAnchors[Top + a] += ResolvedStyle._Anchors[Top + a].Number;
                                absAnchors[Bottom + a] = absAnchors[Top + a] + diffHeight;
                            }
                            else if (ResolvedStyle._Anchors[Bottom + a].HasValue())
                            {
                                normalDirection[a] = false;
                                absAnchors[Bottom + a] -= ResolvedStyle._Anchors[Bottom + a].Number;
                                absAnchors[Top + a] = absAnchors[Bottom + a] - diffHeight;
                            }
                        }
                    }
                }
            }

            MarginRect = absAnchors;
            BorderRect = MarginRect.Substract(relMargin);
            PaddingRect = BorderRect.Substract(relBorder);
            ClientRect = PaddingRect.Substract(relPadding);

            for (var a = axisStart; a < axisCount; a++)
            {
                normalDirection[a] = true;
                var ax = (Axis)a;

                if (ResolvedStyle.MaxSize[a].HasValue() && ClientRect.Size(ax) > relMaxSize[a])
                {
                    var diff = ClientRect.Size(ax) - relMaxSize[a];
                    if (normalDirection[a])
                    {
                        ClientRect[Bottom + a] -= diff;
                        PaddingRect[Bottom + a] -= diff;
                        BorderRect[Bottom + a] -= diff;
                        MarginRect[Bottom + a] -= diff;
                    }
                    else
                    {
                        ClientRect[Top + a] += diff;
                        PaddingRect[Top + a] += diff;
                        BorderRect[Top + a] += diff;
                        MarginRect[Top + a] += diff;
                    }
                }

                if (ResolvedStyle.MinSize[a].HasValue() && ClientRect.Size(ax) < relMinSize[a])
                {
                    var diff = relMinSize[a] - ClientRect.Size(ax);
                    if (normalDirection[a])
                    {
                        ClientRect[Bottom + a] += diff;
                        PaddingRect[Bottom + a] += diff;
                        BorderRect[Bottom + a] += diff;
                        MarginRect[Bottom + a] += diff;
                    }
                    else
                    {
                        ClientRect[Top + a] -= diff;
                        PaddingRect[Top + a] -= diff;
                        BorderRect[Top + a] -= diff;
                        MarginRect[Top + a] -= diff;
                    }
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
