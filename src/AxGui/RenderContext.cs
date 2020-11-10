// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using SkiaSharp;

namespace AxGui
{

    public class RenderContext
    {
        public void Reset()
        {
            Commands.Clear();
            AdditionalCommandLists.Clear();
        }

        public readonly DrawCommands Commands = new DrawCommands();
        public readonly List<DrawCommands> AdditionalCommandLists = new List<DrawCommands>();
    }

}
