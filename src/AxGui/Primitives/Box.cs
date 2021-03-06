﻿// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using SkiaSharp;

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

        public float GetMinMax(Axis ax)
            => ax == Axis.X ? LeftRight : TopBottom;

        public float Width
        {
            get => Right - Left;
            set => Right = Left + value;
        }
        public float Height
        {
            get => Bottom - Top;
            set => Bottom = Top + value;
        }

        public void TranslateX(float value)
        {
            Left += value;
            Right += value;
        }

        public void TranslateY(float value)
        {
            Top += value;
            Bottom += value;
        }

        public void Translate(float x, float y)
        {
            TranslateX(x);
            TranslateY(y);
        }

        public void Translate(Point p)
        {
            TranslateX(p.X);
            TranslateY(p.Y);
        }

        public Point Center
        {
            get => new Point(Left + (Width / 2), Top + (Height / 2));
        }

        public float Size(Axis axis)
        {
            return axis == Axis.X ? Width : Height;
        }

        public Box Substract(float value)
        {
            return new Box(Left + value, Top + value, Right - value, Bottom - value);
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
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
            return $"Left: {Left:F1}, Top: {Top:F1}, Right: {Right:F1}, Bottom: {Bottom:F1}, Width: {Width:F1}, Height: {Height:F1}";
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
        }

        public float this[int index]
        {
            get
            {
                return index switch
                {
                    0 => Left,
                    1 => Top,
                    2 => Right,
                    3 => Bottom,
                    _ => throw new NotSupportedException(),
                };
            }
            set
            {
                _ = index switch
                {
                    0 => Left = value,
                    1 => Top = value,
                    2 => Right = value,
                    3 => Bottom = value,
                    _ => throw new NotSupportedException(),
                };
            }
        }

        public Point Min => new Point(Left, Top);
        public Point Max => new Point(Right, Bottom);

        public Point LeftTop => new Point(Left, Top);
        public Point LeftBottom => new Point(Left, Bottom);

        public Point RightTop => new Point(Right, Top);
        public Point RightBottom => new Point(Right, Bottom);

    }

}
