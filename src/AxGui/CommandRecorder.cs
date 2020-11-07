// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using SkiaSharp;

namespace AxGui
{

    public class CommandRecorder
    {

        internal RenderContext? _RenderContext;

        public void Record(Element root)
        {
            var ctx = root._RenderContext;
            _RenderContext = ctx;

            // set shared values...
            // ...
            //root.Style.Anchors.Bottom.Number = 1;
            root.OnRender(ctx);
        }

    }

}
