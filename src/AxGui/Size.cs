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
                switch (index)
                {
                    case 0:
                        return Width;
                    case 1:
                        return Height;
                    default:
                        throw new ArgumentException(nameof(index));
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        Width = value;
                        break;
                    case 1:
                        Height = value;
                        break;
                    default:
                        throw new ArgumentException(nameof(index));
                }
            }
        }

    }

}
