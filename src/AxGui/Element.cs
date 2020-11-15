﻿// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SkiaSharp;

namespace AxGui
{

    [SkipLocalsInit]
    public class Element : IDisposable
    {
        public readonly ElemenetAttributs Attributes = new ElemenetAttributs();
        public readonly ElementStyle Style = new ElementStyle();
        public readonly ElementStyle ResolvedStyle = new ElementStyle();

        public Element? Parent;

        /// <summary>
        /// Custom data
        /// </summary>
        public object? Data;

        /// <summary>
        /// Logical childs, for example items in a scroll view.
        /// </summary>
        public List<Element> Children;

        /// <summary>
        /// Logical childs and additional childs like the scrollbar of a scroll view.
        /// </summary>
        protected List<Element> ChildrenInternal;
        internal RenderContext RenderContext = new RenderContext();
        internal ProcessLayoutContext ProcessLayoutContext = new ProcessLayoutContext();

        public Element()
        {
            Children = new List<Element>();
            ChildrenInternal = Children;
        }

        public void AddChild(Element el)
        {
            el.Parent = this;
            Children.Add(el);
        }

        /* Order:
         * ComputeStyle
         * ComputeChildBoundsOffers
         * ComputeBounds
         * Render
         */

        protected internal virtual void CallComputeStyle(GlobalProcessLayoutContext ctx)
        {
            var c = ProcessLayoutContext;
            c.GlobalContext = ctx;
            ComputeStyle(c);
        }

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

            ComputeStyleChildren(ctx);
        }

        protected internal virtual void ComputeStyleChildren(ProcessLayoutContext ctx)
        {
            var length = Children.Count;
            var children = Children;
            for (var i = 0; i < length; i++)
                children[i].ComputeStyle(ctx);
        }

        internal Box OuterRect;

        internal Box MarginRect;
        internal Box BorderRect;
        internal Box PaddingRect;
        internal Box ClientRect;
        internal Size NaturalSize;

        private void Translate(float x, float y)
        {
            MarginRect.Translate(x, y);
            BorderRect.Translate(x, y);
            PaddingRect.Translate(x, y);
            ClientRect.Translate(x, y);
        }

        private void TranslateY(float value)
        {
            MarginRect.TranslateY(value);
            BorderRect.TranslateY(value);
            PaddingRect.TranslateY(value);
            ClientRect.TranslateY(value);
        }

        protected internal virtual void CallComputeChildBoundsOffers(GlobalProcessLayoutContext ctx)
        {
            var c = ProcessLayoutContext;
            c.GlobalContext = ctx;
            ComputeChildBoundsOffers(c);
        }

        protected internal virtual void ComputeChildBoundsOffers(ProcessLayoutContext ctx)
        {
        }

        protected internal virtual void CallComputeBounds(GlobalProcessLayoutContext ctx)
        {
            var c = ProcessLayoutContext;
            c.GlobalContext = ctx;
            c.RowPosition = default;
            c.RowHeight = default;
            c.RowElements.Clear();
            ComputeBounds(c);
        }

        protected internal virtual void ComputeBoundsSelf(ProcessLayoutContext ctx)
        {
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

            if (ResolvedStyle.Position == StylePosition.Absolute)
            {
                for (var a = axisStart; a < axisCount; a++)
                {
                    normalDirection[a] = true;
                    var ax = (Axis)a;

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
                            else
                            {
                                absAnchors[Bottom + a] = absAnchors[Top + a] + diffHeight;
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
            else if (ResolvedStyle.Display == StyleDisplay.InlineBlock)
            {
                absAnchors.Width = relSize.Width + decorationSize.Width;
                absAnchors.Height = relSize.Height + decorationSize.Height;

                if (Parent == null)
                    return; // not supported

                var el = GetParentBlockElement(ctx);
                if (el != null)
                {
                    var pc = el.ProcessLayoutContext;
                    if (absAnchors.Right + pc.RowPosition.X > el.ClientRect.Right) // OuterRect.Right
                    {
                        pc.RowElements.Clear();
                        pc.RowPosition.Y += pc.RowHeight;
                        pc.RowPosition.X = 0;
                        pc.RowHeight = absAnchors.Height;
                    }
                    else if (absAnchors.Height > pc.RowHeight)
                    {
                        var diff = absAnchors.Height - pc.RowHeight;
                        var prevElLength = pc.RowElements.Count;
                        for (var i = 0; i < prevElLength; i++)
                        {
                            var prevEl = pc.RowElements[i];
                            prevEl.TranslateY(diff);
                        }
                        pc.RowHeight = absAnchors.Height;
                    }

                    absAnchors.Translate(pc.RowPosition.X, pc.RowPosition.Y + (pc.RowHeight - absAnchors.Height));

                    MarginRect = absAnchors;
                    BorderRect = MarginRect.Substract(relMargin);
                    PaddingRect = BorderRect.Substract(relBorder);
                    ClientRect = PaddingRect.Substract(relPadding);

                    pc.RowPosition.X += absAnchors.Width;
                    //pc.RowHeight = absAnchors.Height;
                    pc.RowElements.Add(this);
                }
            }
        }

        protected internal virtual void ComputeBounds(ProcessLayoutContext ctx)
        {
            ComputeBoundsSelf(ctx);
            ComputeBoundsChildren(ctx);
        }

        protected internal virtual void ComputeBoundsChildren(ProcessLayoutContext ctx)
        {
            var length = Children.Count;
            if (length == 0)
                return;

            var children = Children;
            for (var i = 0; i < length; i++)
            {
                var child = children[i];
                child.ProcessLayoutContext.LocalViewPort = ClientRect;
                child.CallComputeBounds(ctx.GlobalContext!);
            }
        }

        private protected static SKPaint DebugMarginPaint = new SKPaint { Color = new SKColor(174, 129, 82) };
        private protected static SKPaint DebugBorderPaint = new SKPaint { Color = new SKColor(227, 195, 129) };
        private protected static SKPaint DebugPaddingPaint = new SKPaint { Color = new SKColor(183, 196, 127) };
        private protected static SKPaint DebugClientPaint = new SKPaint { Color = new SKColor(135, 178, 188) };

        private protected static SKPaint DebugMarginPaint_Border = new SKPaint { Color = new SKColor(134, 89, 42) };
        private protected static SKPaint DebugBorderPaint_Border = new SKPaint { Color = new SKColor(187, 155, 89) };
        private protected static SKPaint DebugPaddingPaint_Border = new SKPaint { Color = new SKColor(143, 156, 87) };
        private protected static SKPaint DebugClientPaint_Border = new SKPaint { Color = new SKColor(95, 138, 148) };

        private const float DebugBorderWidth = 1;

        protected internal bool Disposed;

        protected internal void CallRender(GlobalRenderContext ctx)
        {
            var c = RenderContext;
            c.GlobalContext = ctx;
            ctx.AddRenderContext(c);
            c.Reset(); // TODO: Flag "AutoReset true/false"
            Render(c);
        }

        protected void RenderBorderAndBackground(RenderContext ctx)
        {
            var marginRectInner = MarginRect.Substract(DebugBorderWidth);
            var borderRectInner = BorderRect.Substract(DebugBorderWidth);
            var paddingRectInner = PaddingRect.Substract(DebugBorderWidth);

            var clientRectInner = ClientRect.Substract(DebugBorderWidth);
            ctx.Commands.Add(new DrawActionCommand(x =>
            {
                x.Canvas.DrawRect(MarginRect.ToSKRect(), DebugMarginPaint_Border);
                x.Canvas.DrawRect(marginRectInner.ToSKRect(), DebugMarginPaint);

                x.Canvas.DrawRect(BorderRect.ToSKRect(), DebugBorderPaint_Border);
                x.Canvas.DrawRect(borderRectInner.ToSKRect(), DebugBorderPaint);

                x.Canvas.DrawRect(PaddingRect.ToSKRect(), DebugPaddingPaint_Border);
                x.Canvas.DrawRect(paddingRectInner.ToSKRect(), DebugPaddingPaint);

                x.Canvas.DrawRect(ClientRect.ToSKRect(), DebugClientPaint_Border);
                x.Canvas.DrawRect(clientRectInner.ToSKRect(), DebugClientPaint);
            }));
        }

        public virtual void Render(RenderContext ctx)
        {
            if (ResolvedStyle.Visibility != StyleVisibility.Hidden)
                RenderBorderAndBackground(ctx);

            RenderChildren(ctx);
        }

        protected internal virtual void RenderChildren(RenderContext ctx)
        {
            var length = Children.Count;
            var children = Children;
            for (var i = 0; i < length; i++)
                children[i].CallRender(ctx.GlobalContext!);
        }

        private protected Box GetParentBlockBox(ProcessLayoutContext ctx)
        {
            var el = GetParentBlockElement(ctx);
            if (el == null)
                return ctx.GlobalContext!.GlobalViewPort;

            return el.ClientRect;
        }

        private protected Element? GetParentBlockElement(ProcessLayoutContext ctx)
        {
            var p = Parent;

            while (p != null)
            {
                if (p.ResolvedStyle.Position == StylePosition.Absolute)
                    return p;

                p = p.Parent;
            }

            return null;
        }

        public override string ToString()
        {
            var label = Data?.ToString();
            if (label != null)
                label = "[" + label + "] ";
            return label + GetType().Name;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            Children = null!;
            Disposed = true;
        }

        ~Element()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}
