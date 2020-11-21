// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace AxGui
{
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct Size : IEquatable<Size>
    {
        public static readonly Size Zero;

        [FieldOffset(0)]
        public float Width;
        [FieldOffset(4)]
        public float Height;

        public Size(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public Size(float value)
        {
            Width = value;
            Height = value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Width, Height);
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is Size m))
                return false;

            return this == m;
        }

        public bool Equals(Size other)
        {
            return this == other;
        }

        public static bool operator ==(Size width, Size height)
        {
            return width.Width == height.Width && width.Height == height.Height;
        }

        public static bool operator !=(Size width, Size height)
        {
            return !(width == height);
        }

        public static Size operator +(Size left, float right)
        {
            return new Size(left.Width + right, left.Height + right);
        }

        public static Size operator -(Size left, float right)
        {
            return new Size(left.Width - right, left.Height - right);
        }

        public static Size operator *(Size left, float right)
        {
            return new Size(left.Width * right, left.Height * right);
        }

        public static Size operator /(Size left, float right)
        {
            return new Size(left.Width / right, left.Height / right);
        }

        public static implicit operator Size(float value)
        {
            return new Size
            {
                Width = value,
                Height = value,
            };
        }

        internal SKSize ToSKSize() => new SKSize(Width, Height);

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
