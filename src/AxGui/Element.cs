// This file is part of AxGUI. Web: https://github.com/AximoGames
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
        private readonly ElementStyle TempStyle = new ElementStyle();
        public readonly ElementStyle ResolvedStyle = new ElementStyle();
        public Point ScrollOffset;

        public Element? Parent;
        private protected bool PassThrough;

        /// <summary>
        /// Custom data
        /// </summary>
        public object? Data;

        public string? CssClass;
        public string? Id;

        /// <summary>
        /// Logical childs, for example items in a scroll view.
        /// </summary>
        internal List<Element> Children;

        /// <summary>
        /// Logical childs and additional childs like the scrollbar of a scroll view.
        /// </summary>
        internal List<Element> ChildrenInternal;
        internal RenderContext RenderContext = new RenderContext();
        private RenderContext PostRenderContext = new RenderContext();
        internal ProcessLayoutContext ProcessLayoutContext = new ProcessLayoutContext();

        public readonly string? TagName;

        public Element() : this(null)
        {
        }

        internal Element(string? tagName)
        {
            Children = new List<Element>();
            ChildrenInternal = Children;
            TagName = tagName;
        }

        public static Element Create(string tagName)
        {
            switch (tagName.ToLower())
            {
                case "span":
                    return new TextElement();
                case "body":
                    return new Element(tagName) { CssClass = "body" };
                default:
                    return new Element(tagName);
            }
        }

        public void AddChild(Element el)
        {
            el.Parent = this;
            Children.Add(el);
        }

        public void SetAttribute(string name, string value)
        {
            switch (name.ToLower())
            {
                case "style":
                    var style = ElementStyle.FromString(value);
                    ElementStyle.Combine(Style, Style, style);
                    break;
                case "class":
                    CssClass = value;
                    break;
                case "id":
                    Id = value;
                    break;
            }
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
            Style.CopyTo(TempStyle);
            var style = TempStyle;
            var resolved = ResolvedStyle;

            var classStyle = ctx.GlobalContext!.Styles!.GetRuleByClass(CssClass);
            var tagStyle = ctx.GlobalContext!.Styles!.GetRuleByTag(TagName);
            var idStyle = ctx.GlobalContext!.Styles!.GetRuleById(Id);

            if (idStyle != null)
                ElementStyle.Combine(style, idStyle.Style, style);

            if (classStyle != null)
                ElementStyle.Combine(style, classStyle.Style, style);

            if (tagStyle != null)
                ElementStyle.Combine(style, tagStyle.Style, style);

            //ResolveStyle(ctx);

            ComputeStyleChildren(ctx);
        }

        protected virtual void ResolveStyle(ProcessLayoutContext ctx)
        {
            var style = TempStyle;
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
            resolved.BorderColor = style.BorderColor;
            resolved.BackgroundColor = style.BackgroundColor;
            resolved.BorderRadius = style.BorderRadius;
        }

        protected internal virtual void ComputeStyleChildren(ProcessLayoutContext ctx)
        {
            var length = Children.Count;
            var children = Children;
            for (var i = 0; i < length; i++)
                children[i].ComputeStyle(ctx);
        }

        //internal Box OuterRect;

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
            ResolveStyle(c);
            ComputeBounds(c);
        }

        protected internal virtual void ComputeBoundsSelf(ProcessLayoutContext ctx)
        {
            if (PassThrough)
                return;

            //var dump = ObjectDumper.Dump(ResolvedStyle);
            //ResolveStyle(ctx);
            //dump = ObjectDumper.Dump(ResolvedStyle);

            //OuterRect = ctx.LocalViewPort;

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

            const int MIN = 0; // Left or Top
            const int MAX = 2; // Right or Bottom

            /*
             *  Anchors: Left, Top, Right, Bottom
             *
             *  position affects position (x, y) and flow of subsequent elements.
             *  it does not affect the dimensions of the box.
             *
             *  if position == static:
             *  must ignore Anchors!
             *
             *  if position == absolute:
             *  MUST ignore Display, except Display.None
             *
             *  Display affects Box-Calculation
             *
             *  If Display == Inline:
             *  MUST ignore Size, take the inner content as it is.
             *
             */

            var decorationSize = new Size(
                relMargin.LeftRight + relBorder.LeftRight + relPadding.LeftRight,
                relMargin.TopBottom + relBorder.TopBottom + relPadding.TopBottom);

            var flow = ResolvedStyle.Position is StylePosition.Static or StylePosition.Relative;

            if (flow)
            {
                // in flow

                if (ResolvedStyle.Display != StyleDisplay.Inline)
                {
                    // StyleDisplay.Inline are typically Text spans, that are converted to
                    // one ore more TextElementFracment / StyleDisplay.InlineBlock

                    if (Parent == null)
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
                        throw new Exception($"Elements with Display.{ResolvedStyle.Display} must be inside of a block element.");
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation

                    var el = GetParentBlockElement(ctx);
                    if (el != null)
                    {
                        Box absAnchors = el.ClientRect;
                        var absCenter = absAnchors.Center;

                        absAnchors.Width = relSize.Width + decorationSize.Width;
                        absAnchors.Height = relSize.Height + decorationSize.Height;

                        var pc = el.ProcessLayoutContext;
                        if (absAnchors.Right + pc.RowPosition.X > el.ClientRect.Right || ResolvedStyle.Display == StyleDisplay.Block) // OuterRect.Right
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
            else // flow == false:
            {

                Box absAnchors = Parent != null ? Parent.ClientRect : ctx.GlobalContext!.GlobalViewPort;
                var absCenter = absAnchors.Center;

                // Because we don't have flow, both axis behave exactly the same,
                // so we can reduce code
                for (var a = axisStart; a < axisCount; a++)
                {
                    normalDirection[a] = true;
                    var ax = (Axis)a;

                    if (ResolvedStyle._Anchors[MIN + a].HasValue() && ResolvedStyle._Anchors[MAX + a].HasValue())
                    {
                        absAnchors[MIN + a] += ResolvedStyle._Anchors[MIN + a].Number;
                        absAnchors[MAX + a] -= ResolvedStyle._Anchors[MAX + a].Number;
                    }
                    else
                    {
                        var diffHeight = relSize[a] + decorationSize[a];
                        if (ResolvedStyle._Margin[MIN + a].Unit == StyleUnit.Auto
                            && ResolvedStyle._Margin[MAX + a].Unit == StyleUnit.Auto)
                        {
                            var size = diffHeight;
                            var halfSize = size / 2;

                            absAnchors[MIN + a] = absCenter[a] - halfSize;
                            absAnchors[MAX + a] = absCenter[a] + halfSize;
                        }
                        else
                        {
                            if (ResolvedStyle._Anchors[MIN + a].HasValue())
                            {
                                absAnchors[MIN + a] += ResolvedStyle._Anchors[MIN + a].Number;
                                absAnchors[MAX + a] = absAnchors[MIN + a] + diffHeight;
                            }
                            else if (ResolvedStyle._Anchors[MAX + a].HasValue())
                            {
                                normalDirection[a] = false;
                                absAnchors[MAX + a] -= ResolvedStyle._Anchors[MAX + a].Number;
                                absAnchors[MIN + a] = absAnchors[MAX + a] - diffHeight;
                            }
                            else
                            {
                                absAnchors[MAX + a] = absAnchors[MIN + a] + diffHeight;
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
                            ClientRect[MAX + a] -= diff;
                            PaddingRect[MAX + a] -= diff;
                            BorderRect[MAX + a] -= diff;
                            MarginRect[MAX + a] -= diff;
                        }
                        else
                        {
                            ClientRect[MIN + a] += diff;
                            PaddingRect[MIN + a] += diff;
                            BorderRect[MIN + a] += diff;
                            MarginRect[MIN + a] += diff;
                        }
                    }

                    if (ResolvedStyle.MinSize[a].HasValue() && ClientRect.Size(ax) < relMinSize[a])
                    {
                        var diff = relMinSize[a] - ClientRect.Size(ax);
                        if (normalDirection[a])
                        {
                            ClientRect[MAX + a] += diff;
                            PaddingRect[MAX + a] += diff;
                            BorderRect[MAX + a] += diff;
                            MarginRect[MAX + a] += diff;
                        }
                        else
                        {
                            ClientRect[MIN + a] -= diff;
                            PaddingRect[MIN + a] -= diff;
                            BorderRect[MIN + a] -= diff;
                            MarginRect[MIN + a] -= diff;
                        }
                    }
                }

            } // flow
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

        internal bool DebugBorders = true;

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
            PostRenderContext.Reset();
            Render(c);

            if (c.AfterChildCommands.Count > 0)
            {
                PostRenderContext.Commands.AddRange(c.AfterChildCommands);
                ctx.AddRenderContext(PostRenderContext);
            }
        }

        private SKPaint Paint = new SKPaint();
        protected void RenderBorderAndBackground(RenderContext ctx)
        {
            if (DebugBorders)
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
            else
            {
                var borderWidth = ResolvedStyle._BorderWidth.Top.Number;
                var borderColor = ResolvedStyle._BorderColor.Top.Color;
                var borderRadius = ResolvedStyle._BorderRadius.TopLeft.Size;
                var bgColor = ResolvedStyle._BackgroundColor.Color;
                var bgRect = ClientRect;

                if (borderRadius == Size.Zero)
                {

                    if (borderWidth > 0 && borderColor.Alpha > 0)
                    {
                        ctx.Commands.Add(new DrawActionCommand(x =>
                        {
                            Paint.Style = SKPaintStyle.Stroke;
                            Paint.StrokeWidth = borderWidth;
                            Paint.Color = borderColor;

                            x.Canvas.DrawRect(BorderRect.Substract(borderWidth / 2).ToSKRect(), Paint);
                        }));
                    }

                    if (ResolvedStyle._BackgroundColor.Unit == StyleUnit.Color)
                    {
                        if (bgColor.Alpha > 0)
                        {
                            ctx.Commands.Add(new DrawActionCommand(x =>
                            {
                                Paint.Style = SKPaintStyle.Fill;
                                Paint.Color = bgColor;
                                x.Canvas.DrawRect(bgRect.ToSKRect(), Paint);
                            }));
                        }
                    }

                }
                else
                {

                    if (bgColor.Alpha > 0)
                    {
                        ctx.Commands.Add(new DrawActionCommand(x =>
                        {
                            Paint.Style = SKPaintStyle.Fill;
                            Paint.Color = bgColor;
                            x.Canvas.DrawRoundRect(ClientRect.ToSKRect(), (borderRadius - (borderWidth / 2)).ToSKSize(), Paint);
                        }));
                    }

                    if (borderWidth > 0 && borderColor.Alpha > 0)
                    {
                        ctx.Commands.Add(new DrawActionCommand(x =>
                        {
                            Paint.Style = SKPaintStyle.Stroke;
                            Paint.StrokeWidth = borderWidth;
                            Paint.Color = borderColor;
                            x.Canvas.DrawRoundRect(BorderRect.Substract(borderWidth / 2).ToSKRect(), borderRadius.ToSKSize(), Paint);
                        }));
                    }
                }

            }
        }

        public virtual void Render(RenderContext ctx)
        {
            if (ResolvedStyle.Visibility != StyleVisibility.Hidden)
                RenderBorderAndBackground(ctx);

            RenderChildren(ctx);
        }

        protected internal virtual void RenderChildren(RenderContext ctx)
        {
            var scroll = ScrollOffset != Point.Zero;
            if (scroll)
            {
                ctx.Commands.Add(new DrawActionCommand(x =>
                {
                    x.Canvas.Save();
                    x.Canvas.ClipRect(ClientRect.ToSKRect());
                    x.Canvas.Translate(new SKPoint(-ScrollOffset.X, -ScrollOffset.Y));
                }));
            }

            var length = Children.Count;
            var children = Children;
            for (var i = 0; i < length; i++)
                children[i].CallRender(ctx.GlobalContext!);

            if (scroll)
            {
                ctx.AfterChildCommands.Add(new DrawActionCommand(x =>
                {
                    x.Canvas.Restore();
                }));
            }
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

            if (!string.IsNullOrEmpty(TagName))
                label = "<" + TagName + (string.IsNullOrEmpty(CssClass) ? "" : " ." + CssClass) + "> " + label;

            return label + GetType().Name;
        }

        #region Disposing

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

        #endregion

        internal IEnumerable<Element> GetNodesWithSelf()
        {
            yield return this;

            foreach (var child in Children)
                foreach (var itm in child.GetNodesWithSelf())
                    yield return itm;
        }

    }

}
