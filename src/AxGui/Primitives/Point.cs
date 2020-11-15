// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace AxGui
{
    public struct Point
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float this[int index]
        {
            get
            {
                return index switch
                {
                    0 => X,
                    1 => Y,
                    _ => throw new NotSupportedException(),
                };
            }
            set
            {
                _ = index switch
                {
                    0 => X = value,
                    1 => Y = value,
                    _ => throw new NotSupportedException(),
                };
            }
        }

        public override string ToString()
        {
            return $"X={X.ToString("F1", CultureInfo.InvariantCulture)} Y={Y.ToString("F1", CultureInfo.InvariantCulture)}";
        }

    }

}
