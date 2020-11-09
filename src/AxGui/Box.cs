// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SkiaSharp;
using System;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace AxGui
{

    public struct Box
    {
        public float Left;
        public float Top;
        public float Right;
        public float Bottom;

        public Box(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public float TopBottom => Top + Bottom;
        public float LeftRight => Left + Right;

        public float Width => Right - Left;
        public float Height => Bottom - Top;

        public Point Center
        {
            get => new Point(Right + (Width / 2), Top + (Height / 2));
        }

        public float Size(Axis axis)
        {
            return axis == Axis.X ? Width : Height;
        }

        public Box Substract(Box other)
        {
            return new Box(Left + other.Left, Top + other.Top, Right - other.Right, Bottom - other.Bottom);
        }

        public SKRect ToSKRect()
        {
            return new SKRect(Left, Top, Right, Bottom);
        }

        public override string ToString()
        {
            return $"Left: {Left:F1}, Top: {Top:F1}, Right: {Right:F1}, Bottom: {Bottom:F1}, Width: {Width:F1}, Height: {Height:F1}";
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return Left;
                    case 1:
                        return Top;
                    case 2:
                        return Right;
                    case 3:
                        return Bottom;
                    default:
                        throw new ArgumentException(nameof(index));
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        Left = value;
                        break;
                    case 1:
                        Top = value;
                        break;
                    case 2:
                        Right = value;
                        break;
                    case 3:
                        Bottom = value;
                        break;
                    default:
                        throw new ArgumentException(nameof(index));
                }
            }
        }

    }

}
