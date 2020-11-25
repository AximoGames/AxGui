// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using SkiaSharp;

namespace AxGui
{

    public class Document
    {
        internal Element _Body;
        public Element Body
        {
            get => _Body;
            set
            {
                _Body = value;
                GetElementByIdEnumerator = new ElementEnumerator(_Body);
            }
        }

        public StyleCollection Styles = new StyleCollection();

        public Document()
        {
            _Body = new Element("body");
            GetElementByIdEnumerator = new ElementEnumerator(_Body);
        }

        public IEnumerable<Element> All => _Body.GetNodesWithSelf();

        private IEnumerator<Element> GetElementByIdEnumerator;
        public Element? GetElementById(string id)
        {
            GetElementByIdEnumerator.Reset();
            while (GetElementByIdEnumerator.MoveNext())
            {
                if (GetElementByIdEnumerator.Current.Id == id)
                    return GetElementByIdEnumerator.Current;
            }
            return null;
        }

        public static Document FromString(string content)
        {
            using var fs = new StringReader(content);
            return FromStream(fs);
        }

        public static Document FromFile(string path)
        {
            using var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return FromStream(fs);
        }

        public static Document FromStream(Stream stream)
        {
            using var reader = new StreamReader(stream);
            return FromStream(reader);
        }

        public static Document FromStream(TextReader reader)
        {
            Sgml.SgmlReader sgmlReader = new Sgml.SgmlReader();
            sgmlReader.DocType = "HTML";
            sgmlReader.WhitespaceHandling = WhitespaceHandling.All;
            sgmlReader.CaseFolding = Sgml.CaseFolding.ToLower;
            sgmlReader.InputStream = reader;

            // create document
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.XmlResolver = null;
            doc.Load(sgmlReader);

            if (doc.FirstChild == null)
                return new Document();

            return FromXml(XDocument.Parse(doc.OuterXml));

            //return FromStream(sgmlReader);
        }

        public static Document FromStream(XmlReader reader)
        {
            // create document
            XDocument doc = (XDocument)XNode.ReadFrom(reader);
            return FromXml(doc);
        }

        public static Document FromXml(XDocument doc)
        {
            var d = new Document();
            var root = new Element("html");
            FromXml(d, doc.Elements().First(), root);

            var b = root.Children.FirstOrDefault(c => c.TagName == "body");
            if (b != null)
            {
                d.Body = b;
            }
            else
            {
                foreach (var child in root.Children)
                {
                    d._Body.AddChild(child);
                }
            }

            return d;
        }

        private static void FromXml(Document d, XElement xmlParent, Element parent)
        {
            foreach (var node in xmlParent.Nodes())
            {
                if (node is XElement xmlEl)
                {
                    var tagName = xmlEl.Name.LocalName;
                    if (tagName.ToLower() == "style")
                    {
                        var styleContent = (xmlEl.FirstNode as XCData)?.Value;
                        var collection = StyleCollection.FromString(styleContent);
                        d.Styles.Rules.AddRange(collection.Rules);
                    }
                    else
                    {
                        var el = Element.Create(tagName);

                        foreach (var attr in xmlEl.Attributes())
                            el.SetAttribute(attr.Name.LocalName, attr.Value);

                        parent.AddChild(el);
                        FromXml(d, xmlEl, el);
                    }
                }
                else if (node is XText t)
                {
                    //if (parent is TextElement pt)
                    //{
                    //    pt.Content = t.Value;
                    //}
                    //else
                    //{
                    //    var el = new TextElement()
                    //    {
                    //        Content = t.Value,
                    //    };
                    //    parent.AddChild(el);
                    //}
                }
            }
        }

    }

}
