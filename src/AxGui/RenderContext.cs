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

        internal bool HasCommands => Commands.Count > 0 || AdditionalCommandLists.Count > 0;

        public bool DebugBorders;
        public readonly DrawCommands Commands = new DrawCommands();
        public readonly DrawCommands AfterChildCommands = new DrawCommands();
        public readonly List<DrawCommands> AdditionalCommandLists = new List<DrawCommands>();
        internal GlobalRenderContext? GlobalContext;

        public override string ToString()
        {
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
            return $"Commands: {Commands.Count}, AdditionalCommands: {AdditionalCommandLists.Count}";
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
        }

    }

}
