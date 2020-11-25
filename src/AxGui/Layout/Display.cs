// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using SkiaSharp;
using System.IO;
using System.Text;

namespace AxGui
{

    internal class LegacyDisplay : Display
    {
        public LegacyDisplay(Element element) : base(element)
        {
        }
    }

    internal abstract class Display
    {

        private protected bool PassThrough;
        public Point ScrollOffset;

        internal RenderContext RenderContext => Element.RenderContext;
        internal RenderContext PostRenderContext => Element.PostRenderContext;
        internal ProcessLayoutContext ProcessLayoutContext => Element.ProcessLayoutContext;
        internal ElementStyle Style => Element.Style;
        internal ElementStyle TempStyle => Element.TempStyle;
        internal ElementStyle ResolvedStyle => Element.ResolvedStyle;
        internal string? CssClass => Element.CssClass;
        internal string? Id => Element.Id;
        internal string? TagName => Element.TagName;
        internal List<Element> Children => Element.Children;
        internal List<Element> ChildrenInternal => Element.ChildrenInternal;
        internal Element? Parent => Element.Parent;

        public Display(Element element)
        {
            Element = element;
        }

        public Element Element;
        //public Box PrefClient;

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
            ComputeStyleSelf(ctx);
            ComputeStyleChildren(ctx);
        }

        protected internal virtual void ComputeStyleSelf(ProcessLayoutContext ctx)
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

            resolved._BorderColor = style._BorderColor;
            resolved._BackgroundColor = style._BackgroundColor;
            resolved._BorderRadius = style._BorderRadius;
            resolved._Color = style._Color;
            resolved._FontSize = style._FontSize;
            resolved._FontFamily = style._FontFamily;

            resolved.Position = style.Position;
            resolved.Display = style.Display;
            resolved.Visibility = style.Visibility;

            resolved.JustifyContent = style.JustifyContent;
            resolved.AlignItems = style.AlignItems;
            resolved.FlexDirection = style.FlexDirection;
            resolved._FlexGrow = style._FlexGrow;

            if (resolved.FlexDirection == StyleFlexDirection.Unset)
                resolved.FlexDirection = StyleFlexDirection.Row;
        }

        protected internal virtual void ComputeStyleChildren(ProcessLayoutContext ctx)
        {
            var length = Children.Count;
            var children = Children;
            for (var i = 0; i < length; i++)
                children[i].Display.ComputeStyle(ctx);
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

        private void TranslateChilds(float x, float y)
        {
            var childs = Children;
            var len = childs.Count;
            for (var i = 0; i < len; i++)
                childs[i].Display.TranslateWithChilds(x, y);
        }

        private void TranslateWithChilds(float x, float y)
        {
            Translate(x, y);

            var childs = Children;
            var len = childs.Count;
            for (var i = 0; i < len; i++)
                childs[i].Display.TranslateWithChilds(x, y);
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
                        Box absAnchors = el.Display.ClientRect;
                        var absCenter = absAnchors.Center;

                        if (Parent.ResolvedStyle.Display == StyleDisplay.Flex
                            && ResolvedStyle.FlexGrow.Number > 0
                            && Parent.ResolvedStyle.FlexDirection == StyleFlexDirection.Row)
                            absAnchors.Width = el.Display.ClientRect.Width - GetConsumedParentSize(ctx).Width; // Possible problem: it can still break, if it's 0.00001 px larger.
                        else if (!ResolvedStyle.Display.IsBlock() || ResolvedStyle.Width.Unit != StyleUnit.Unset)
                            absAnchors.Width = relSize.Width + decorationSize.Width;

                        if (Parent.ResolvedStyle.Display == StyleDisplay.Flex
                            && ResolvedStyle.FlexGrow.Number > 0
                            && Parent.ResolvedStyle.FlexDirection == StyleFlexDirection.Column)
                            absAnchors.Height = el.Display.ClientRect.Height - GetConsumedParentSize(ctx).Height; // Possible problem: it can still break, if it's 0.00001 px larger.
                        else
                            absAnchors.Height = relSize.Height + decorationSize.Height;

                        var pc = el.ProcessLayoutContext;
                        if (absAnchors.Right + pc.RowPosition.X > el.Display.ClientRect.Right
                            || (ResolvedStyle.Display.IsBlock()
                            && ResolvedStyle.FlexDirection == StyleFlexDirection.Row)) // OuterRect.Right
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
                                prevEl.Display.TranslateY(diff);
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
                        pc.RowElements.Add(Element);
                    }
                }

                Point translate = default;
                if (ResolvedStyle._Anchors._Left.HasValue())
                    translate.X += ResolvedStyle._Anchors._Left.Number;
                else if (ResolvedStyle._Anchors._Right.HasValue())
                    translate.X -= ResolvedStyle._Anchors._Right.Number;

                if (ResolvedStyle._Anchors._Top.HasValue())
                    translate.Y += ResolvedStyle._Anchors._Top.Number;
                else if (ResolvedStyle._Anchors._Bottom.HasValue())
                    translate.X -= ResolvedStyle._Anchors._Bottom.Number;

                if (translate != Point.Zero)
                    Translate(translate.X, translate.Y);
            }
            else // flow == false:
            {

                Box absAnchors = Parent != null ? Parent.Display.ClientRect : ctx.GlobalContext!.GlobalViewPort;
                if (ResolvedStyle.Position == StylePosition.Absolute)
                {
                    var parentAnchors = GetRelativeParentRect(ctx);
                    if (ResolvedStyle._Anchors._Top.HasValue() || ResolvedStyle._Anchors._Bottom.HasValue())
                    {
                        absAnchors.Top = parentAnchors.Top;
                        absAnchors.Bottom = parentAnchors.Bottom;
                    }

                    if (ResolvedStyle._Anchors._Left.HasValue() || ResolvedStyle._Anchors._Right.HasValue())
                    {
                        absAnchors.Left = parentAnchors.Left;
                        absAnchors.Right = parentAnchors.Right;
                    }
                }

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
            JustifyChildren(ctx);
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
                if (child.TempStyle.Position == StylePosition.Absolute)
                {
                    var parent = child.Display.GetRelativeParent(ctx);
                    var viewPort = child.Display.ClientRect;
                    if (viewPort.Height == 0 || viewPort.Width == 0)
                    {
                        if (parent != null)
                        {
                            var realSize = parent.Children[0].Display.MarginRect; // TODO: Fix that hack!
                            if (viewPort.Height == 0)
                            {
                                viewPort.Top = realSize.Top;
                                viewPort.Bottom = realSize.Bottom;
                            }

                            if (viewPort.Width == 0)
                            {
                                viewPort.Left = realSize.Left;
                                viewPort.Right = realSize.Right;
                            }
                            //throw new NotImplementedException("not supported yet. Try explicit size in the mean time");
                        }
                    }
                    if (parent != null)
                    {
                        var tmpViewPort = viewPort;
                        viewPort = parent.Display.PaddingRect;
                        viewPort.Bottom += tmpViewPort.Height;
                        viewPort.Right += tmpViewPort.Right;
                    }
                    else
                    {
                        viewPort = ClientRect;
                    }

                    child.ProcessLayoutContext.LocalViewPort = viewPort;
                }
                else
                {
                    child.ProcessLayoutContext.LocalViewPort = ClientRect;
                }
                child.Display.CallComputeBounds(ctx.GlobalContext!);
            }
        }

        private protected Box GetRelativeParentRect(ProcessLayoutContext ctx)
        {
            var el = GetRelativeParent(ctx);
            if (el == null)
                return ctx.GlobalContext!.GlobalViewPort;

            return el.Display.ClientRect;
        }

        private protected Element? GetRelativeParent(ProcessLayoutContext ctx)
        {
            var p = Parent;

            while (p != null)
            {
                if (!p.Display.PassThrough && p.Style.Position == StylePosition.Relative)
                    return p;

                p = p.Parent;
            }

            return null;
        }

        protected internal virtual void JustifyChildren(ProcessLayoutContext ctx)
        {
            var childBounds = GetChildsBounds();

            // grow surrounding div
            if (ResolvedStyle.Height.Unit == StyleUnit.Unset && (ResolvedStyle.FlexGrow.Unit == StyleUnit.Unset || ResolvedStyle.FlexGrow.Number == 0))
            {
                ClientRect.Height += childBounds.Height;
                PaddingRect.Height += childBounds.Height;
                BorderRect.Height += childBounds.Height;
                MarginRect.Height += childBounds.Height;

                if (ResolvedStyle.Display.IsBlock())
                {
                    // shift Descendant flow
                    var el = GetParentBlockElement(ctx);
                    if (el != null)
                        el.ProcessLayoutContext.RowHeight += childBounds.Height;
                }
            }

            if (ResolvedStyle.Display.IsFlex())
            {
                if (ResolvedStyle.JustifyContent == StyleJustifyContenet.Center)
                {
                    var offset = new Point();

                    var space = ClientRect.Width - childBounds.Width;
                    if (space > 0)
                        offset.X = space / 2;

                    space = ClientRect.Height - childBounds.Height;
                    if (space > 0)
                        offset.Y = space / 2;

                    if (offset != Point.Zero)
                        TranslateChilds(offset.X, offset.Y);
                }
            }
        }

        private Size GetConsumedParentSize(ProcessLayoutContext ctx)
        {
            if (Parent == null)
                return default;

            var childs = Parent.Children;
            var len = childs.Count;
            if (len == 0)
                return default;

            Size result = default;
            for (var i = 0; i < len; i++)
            {
                var child = childs[i];
                if (child == Element)
                    continue;
                var style = child.TempStyle;

                //var size = style.Size.Normalize(ctx);

                //result.Width += style.Margin.Left.Normalize(ctx, Axis.X).Number;
                //result.Width += style.Margin.Right.Normalize(ctx, Axis.X).Number;
                //result.Height += style.Margin.Top.Normalize(ctx, Axis.Y).Number;
                //result.Height += style.Margin.Bottom.Normalize(ctx, Axis.Y).Number;

                //result.Width += style.BorderWidth.Left.Normalize(ctx, Axis.X).Number;
                //result.Width += style.BorderWidth.Right.Normalize(ctx, Axis.X).Number;
                //result.Height += style.BorderWidth.Top.Normalize(ctx, Axis.Y).Number;
                //result.Height += style.BorderWidth.Bottom.Normalize(ctx, Axis.Y).Number;

                //result.Width += style.Padding.Left.Normalize(ctx, Axis.X).Number;
                //result.Width += style.Padding.Right.Normalize(ctx, Axis.X).Number;
                //result.Height += style.Padding.Top.Normalize(ctx, Axis.Y).Number;
                //result.Height += style.Padding.Bottom.Normalize(ctx, Axis.Y).Number;

                //result.Width += style.Width.Normalize(ctx, Axis.X).Number;
                //result.Height += style.Height.Normalize(ctx, Axis.Y).Number;

                result.Width += child.Display.MarginRect.Width;
                result.Height += child.Display.MarginRect.Height;
            }

            return result;
        }

        private Box GetChildsBounds()
        {
            var childs = Children;
            var len = childs.Count;
            if (len == 0)
                return default;

            Box box = childs[0].Display.GetBorderBounds();
            for (var i = 1; i < len; i++)
                box = box.Union(childs[i].Display.GetBorderBounds());

            return box;
        }

        private Box GetBorderBounds()
        {
            if (!PassThrough)
                return MarginRect;

            return GetChildsBounds();
        }

        private protected Box GetParentBlockBox(ProcessLayoutContext ctx)
        {
            var el = GetParentBlockElement(ctx);
            if (el == null)
                return ctx.GlobalContext!.GlobalViewPort;

            return el.Display.ClientRect;
        }

        private protected Element? GetParentBlockElement(ProcessLayoutContext ctx)
        {
            var p = Parent;

            while (p != null)
            {
                if (!p.Display.PassThrough)
                    return p;

                p = p.Parent;
            }

            return null;
        }

        private const byte DebugAlpha = 255;
        private protected static SKPaint DebugMarginPaint = new SKPaint { Color = new SKColor(174, 129, 82, DebugAlpha) };
        private protected static SKPaint DebugBorderPaint = new SKPaint { Color = new SKColor(227, 195, 129, DebugAlpha) };
        private protected static SKPaint DebugPaddingPaint = new SKPaint { Color = new SKColor(183, 196, 127, DebugAlpha) };
        private protected static SKPaint DebugClientPaint = new SKPaint { Color = new SKColor(135, 178, 188, DebugAlpha) };

        private protected static SKPaint DebugMarginPaint_Border = new SKPaint { Color = new SKColor(134, 89, 42, DebugAlpha) };
        private protected static SKPaint DebugBorderPaint_Border = new SKPaint { Color = new SKColor(187, 155, 89, DebugAlpha) };
        private protected static SKPaint DebugPaddingPaint_Border = new SKPaint { Color = new SKColor(143, 156, 87, DebugAlpha) };
        private protected static SKPaint DebugClientPaint_Border = new SKPaint { Color = new SKColor(95, 138, 148, DebugAlpha) };

        private const float DebugBorderWidth = 1;

        protected internal void CallRender(GlobalRenderContext ctx)
        {
            var c = RenderContext;
            c.GlobalContext = ctx;
            c.DebugBorders = ctx.DebugBorders;
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
            if (ctx.DebugBorders)
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
                var bgRect = PaddingRect;

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
                            x.Canvas.DrawRoundRect(bgRect.ToSKRect(), (borderRadius - (borderWidth / 2)).ToSKSize(), Paint);
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
                children[i].Display.CallRender(ctx.GlobalContext!);

            if (scroll)
            {
                ctx.AfterChildCommands.Add(new DrawActionCommand(x =>
                {
                    x.Canvas.Restore();
                }));
            }
        }

    }

}
