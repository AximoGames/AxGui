﻿// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using SkiaSharp;

namespace AxGui
{

    public class CommandRecorder
    {

        internal readonly GlobalRenderContext RenderContext = new GlobalRenderContext();

        public void Record(Element root)
        {
            root.CallRender(RenderContext);
        }

    }

}
