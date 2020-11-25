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

    [SkipLocalsInit]
    public class Element : IDisposable
    {
        public readonly ElemenetAttributs Attributes = new ElemenetAttributs();
        public readonly ElementStyle Style = new ElementStyle();
        internal readonly ElementStyle TempStyle = new ElementStyle();
        public readonly ElementStyle ResolvedStyle = new ElementStyle();

        public Element? Parent;

        internal Display Display;

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
        internal RenderContext PostRenderContext = new RenderContext();
        internal ProcessLayoutContext ProcessLayoutContext;

        public readonly string? TagName;

        public Box MarginRect => Display.MarginRect;
        public Box BorderRect => Display.BorderRect;
        public Box PaddingRect => Display.PaddingRect;
        public Box ClientRect => Display.ClientRect;

        public Element() : this(null)
        {
        }

        internal Element(string? tagName)
        {
            Children = new List<Element>();
            ChildrenInternal = Children;
            TagName = tagName;
            ProcessLayoutContext = new ProcessLayoutContext(this);
            Display = new LegacyDisplay(this);
        }

        public static Element Create(string tagName)
        {
            switch (tagName.ToLower())
            {
                //case "span":
                //    return new TextElement();
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

        protected internal void CallRender(GlobalRenderContext ctx)
        {
            Display.CallRender(ctx);
        }

        protected internal virtual void CallComputeStyle(GlobalProcessLayoutContext ctx)
        {
            Display.CallComputeStyle(ctx);
        }

        protected internal virtual void CallComputeBounds(GlobalProcessLayoutContext ctx)
        {
            Display.CallComputeBounds(ctx);
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

        protected internal bool Disposed;
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

        public string ToHtml()
        {
            return ToXml().ToString(SaveOptions.DisableFormatting);
        }

        public XElement ToXml()
        {
            var el = new XElement(TagName ?? "div");
            foreach (var entry in Attributes)
                if (entry.Key != "style" && !string.IsNullOrEmpty(entry.Value))
                    el.SetAttributeValue(entry.Key, entry.Value);

            var parser = new ExCSS.StylesheetParser(includeUnknownDeclarations: true, tolerateInvalidConstraints: true, tolerateInvalidValues: true);
            var rule = new ExCSS.StyleRule(parser);
            var decl = rule.Style;
            ElementStyle.WriteRule(decl, Style);
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            decl.ToCss(writer, ExCSS.CompressedStyleFormatter.Instance);
            var str = sb.ToString();
            if (!string.IsNullOrEmpty(str))
                el.SetAttributeValue("style", str);

            foreach (var child in Children)
                el.Add(child.ToXml());

            return el;
        }

    }

}
