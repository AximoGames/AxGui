// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace AxGui
{
    public struct Size
    {
        public float Width;
        public float Height;
        public Size(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public float this[int index]
        {
            get
            {
                return index switch
                {
                    0 => Width,
                    1 => Height,
                    _ => throw new NotSupportedException(),
                };
            }
            set
            {
                _ = index switch
                {
                    0 => Width = value,
                    1 => Height = value,
                    _ => throw new NotSupportedException(),
                };
            }
        }

    }

}
