// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using SkiaSharp;

namespace AxGui
{

    public class Document
    {
        public Element Body = new Element();
        public StyleCollection Styles = new StyleCollection();

        public static Document FromFile(string path)
        {
            using var fs = File.OpenRead(path);
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
            var root = new Element();
            FromXml(d, doc.Elements().First(), root);
            return d;
        }

        private static void FromXml(Document d, XElement xmlParent, Element parent)
        {
            foreach (var xmlEl in xmlParent.Elements())
            {
                var el = new Element();
                parent.AddChild(el);

                FromXml(d, xmlEl, el);
            }
        }

    }

}
