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

    internal abstract class Display
    {
        public Display(Element element)
        {
            Element = element;
        }

        internal Element Element;

        protected internal bool PassThrough;
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

        internal Box MarginRect;
        internal Box BorderRect;
        internal Box PaddingRect;
        internal Box ClientRect;
        internal Size NaturalSize;

        protected internal abstract void CallRender(GlobalRenderContext ctx);

        protected internal void Translate(float x, float y)
        {
            MarginRect.Translate(x, y);
            BorderRect.Translate(x, y);
            PaddingRect.Translate(x, y);
            ClientRect.Translate(x, y);
        }

        protected internal void TranslateY(float value)
        {
            MarginRect.TranslateY(value);
            BorderRect.TranslateY(value);
            PaddingRect.TranslateY(value);
            ClientRect.TranslateY(value);
        }

        protected internal void TranslateChilds(float x, float y)
        {
            var childs = Children;
            var len = childs.Count;
            for (var i = 0; i < len; i++)
                childs[i].Display.TranslateWithChilds(x, y);
        }

        protected internal void TranslateWithChilds(float x, float y)
        {
            Translate(x, y);

            var childs = Children;
            var len = childs.Count;
            for (var i = 0; i < len; i++)
                childs[i].Display.TranslateWithChilds(x, y);
        }

        protected internal abstract void CallComputeBounds(GlobalProcessLayoutContext ctx);

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

        protected internal virtual void ComputeStyleChildren(ProcessLayoutContext ctx)
        {
            var length = Children.Count;
            var children = Children;
            for (var i = 0; i < length; i++)
                children[i].Display.ComputeStyle(ctx);
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

        protected internal abstract Box GetBorderBounds();

        protected internal abstract Box GetParentBlockBox(ProcessLayoutContext ctx);

        protected internal abstract Box GetRelativeParentRect(ProcessLayoutContext ctx);

        protected internal abstract Element? GetRelativeParent(ProcessLayoutContext ctx);

    }

}
