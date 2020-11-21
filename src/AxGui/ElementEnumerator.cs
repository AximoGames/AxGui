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

#nullable disable

namespace AxGui
{

    internal class ElementEnumerator : IEnumerator<Element>
    {

        private IEnumerator<Element> Inner;
        private Element Root;
        public ElementEnumerator(Element root)
        {
            Root = root;
            Reset();
        }

        public Element Current => Inner.Current;

        object IEnumerator.Current => Inner.Current;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            return Inner.MoveNext();
        }

        public void Reset()
        {
            Inner = Root.GetNodesWithSelf().GetEnumerator();
        }
    }

}
