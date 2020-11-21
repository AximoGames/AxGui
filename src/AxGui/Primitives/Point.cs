// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace AxGui
{
    public struct Point : IEquatable<Point>
    {
        public float X { get; set; }
        public float Y { get; set; }

        public static readonly Point Zero;

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, X);
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is Point m))
                return false;

            return this == m;
        }

        public bool Equals(Point other)
        {
            return this == other;
        }

        public static bool operator ==(Point x, Point y)
        {
            return x.X == y.X && x.Y == y.Y;
        }

        public static bool operator !=(Point x, Point y)
        {
            return !(x == y);
        }

        public static implicit operator Point(float value)
        {
            return new Point
            {
                Y = value,
                X = value,
            };
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
