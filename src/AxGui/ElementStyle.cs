﻿// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace AxGui
{

    public enum StyleDisplay
    {
        Unset,
        Initial,
        Inherit,

        Block,
        InlineBlock,
        Inline,
        None,
    }

    public enum StyleVisibility
    {
        Unset,
        Initial,
        Inherit,

        Visible,
        Hidden,
    }

    public enum StylePosition
    {
        Unset,
        Initial,
        Inherit,

        Static,
        Relative,
        Absolute,
        Fixed,
    }

    public enum BoxSizing
    {
        Unset,
        Initial,
        Inherit,

        ContentBox,
        BorderBox,
    }

    public class ElementStyle
    {

        public ElementStyle()
        {
            BoundingChangedDelegate = BoundingChanged;
            _Margin.ParentChanged = BoundingChangedDelegate;
            _Padding.ParentChanged = BoundingChangedDelegate;
            _BorderWidth.ParentChanged = BoundingChangedDelegate;
            _Anchors.ParentChanged = BoundingChangedDelegate;
        }

        public void CopyTo(ElementStyle target)
        {
            target._BoundingChanged = _BoundingChanged;

            target._Anchors = _Anchors;
            target._Margin = _Margin;
            target._BorderColor = _BorderColor;
            target._BorderStyle = _BorderStyle;
            target._BorderWidth = _BorderWidth;
            target._Padding = _Padding;

            target._MaxSize = _MaxSize;
            target._MinSize = _MinSize;
            target._Size = _Size;

            target._BorderRadius = _BorderRadius;

            target.Display = Display;
            target.Position = Position;
            target.Visibility = Visibility;
        }

        public static ElementStyle Combine(ElementStyle parent, ElementStyle child)
        {
            var target = new ElementStyle();
            Combine(target, parent, child);
            return target;
        }

        public static void Combine(ElementStyle target, ElementStyle parent, ElementStyle child)
        {
            target._BoundingChanged = true;

            target._Anchors = BoxModelRect.Combine(parent._Anchors, child._Anchors);
            target._Margin = BoxModelRect.Combine(parent._Margin, child._Margin);
            target._BorderColor = BoxModelRect.Combine(parent._BorderColor, child._BorderColor);
            target._BorderWidth = BoxModelRect.Combine(parent._BorderWidth, child._BorderWidth);
            target._BorderStyle = BoxModelRect.Combine(parent._BorderStyle, child._BorderStyle);
            target._Padding = BoxModelRect.Combine(parent._Padding, child._Padding);

            target._MaxSize = StyleSize.Combine(parent._MaxSize, child._MaxSize);
            target._MinSize = StyleSize.Combine(parent._MinSize, child._MinSize);
            target._Size = StyleSize.Combine(parent._Size, child._Size);

            target._BorderRadius = StyleValue.Combine(parent._BorderRadius, child._BorderRadius);

            // TODO: Use StyleValue! For example StyleValue.As<StylePosition>().
            target.Display = child.Display;
            target.Position = child.Position;
            target.Visibility = child.Visibility;
        }

        internal static void SetRule(ElementStyle target, ExCSS.StyleDeclaration parent)
        {
            target._BoundingChanged = true;

            target._Anchors = new BoxModelRect(parent.Left, parent.Top, parent.Right, parent.Bottom);
            target._Margin = BoxModelRect.Combine(parent.Margin, new BoxModelRect(parent.MarginLeft, parent.MarginTop, parent.MarginRight, parent.MarginBottom));
            target._BorderColor = BoxModelRect.Combine(parent.BorderColor, new BoxModelRect(parent.BorderLeftColor, parent.BorderTopColor, parent.BorderRightColor, parent.BorderBottomColor));
            target._BorderWidth = BoxModelRect.Combine(parent.BorderWidth, new BoxModelRect(parent.BorderLeftWidth, parent.BorderTopWidth, parent.BorderRightWidth, parent.BorderBottomWidth));
            target._BorderStyle = BoxModelRect.Combine(parent.BorderStyle, new BoxModelRect(parent.BorderLeftStyle, parent.BorderTopStyle, parent.BorderRightStyle, parent.BorderBottomStyle));
            target._Padding = BoxModelRect.Combine(parent.Padding, new BoxModelRect(parent.PaddingLeft, parent.PaddingTop, parent.PaddingRight, parent.PaddingBottom));

            target._MaxSize = new StyleSize(parent.MaxWidth, parent.MaxHeight);
            target._MinSize = new StyleSize(parent.MinWidth, parent.MinHeight);
            target._Size = new StyleSize(parent.Width, parent.Height);

            target._BorderRadius = parent.BorderRadius;

            // TODO: Use StyleValue! For example StyleValue.As<StylePosition>().
            //target.Display = child.Display;
            //target.Position = child.Position;
            //target.Visibility = child.Visibility;
        }

        internal bool _BoundingChanged;
        private void BoundingChanged()
        {
            _BoundingChanged = true;
        }

        private Action BoundingChangedDelegate;

        // TODO: Use StyleValue! For example StyleValue.As<StylePosition>().
        public StylePosition Position;
        public StyleDisplay Display;
        public StyleVisibility Visibility;

        internal StyleSize _Size;
        public StyleSize Size
        {
            get => _Size;
            set
            {
                if (_Size == value)
                    return;
                _Size = value;
                BoundingChanged();
            }
        }

        public StyleValue Width
        {
            get => _Size.Width;
            set => Size = new StyleSize(value, _Size.Height);
        }

        public StyleValue Height
        {
            get => _Size.Height;
            set => Size = new StyleSize(_Size.Width, value);
        }

        internal StyleSize _MinSize;
        public StyleSize MinSize
        {
            get => _MinSize;
            set
            {
                if (_MinSize == value)
                    return;
                _MinSize = value;
                BoundingChanged();
            }
        }

        public StyleValue MinWidth
        {
            get => _MinSize.Width;
            set => MinSize = new StyleSize(value, _MinSize.Height);
        }

        public StyleValue MinHeight
        {
            get => _MinSize.Height;
            set => MinSize = new StyleSize(_MinSize.Width, value);
        }

        internal StyleSize _MaxSize;
        public StyleSize MaxSize
        {
            get => _MaxSize;
            set
            {
                if (_MaxSize == value)
                    return;
                _MaxSize = value;
                BoundingChanged();
            }
        }

        public StyleValue MaxWidth
        {
            get => _MaxSize.Width;
            set => MaxSize = new StyleSize(value, _MaxSize.Height);
        }

        public StyleValue MaxHeight
        {
            get => _MaxSize.Height;
            set => MaxSize = new StyleSize(_MaxSize.Width, value);
        }

        internal BoxModelRect _Anchors;
        public BoxModelRect Anchors
        {
            get => _Anchors;
            set
            {
                if (_Anchors == value)
                    return;
                _Anchors = value;
                _Anchors.ParentChanged = BoundingChangedDelegate;
                BoundingChanged();
            }
        }

        internal BoxModelRect _Margin;
        public BoxModelRect Margin
        {
            get => _Margin;
            set
            {
                if (_Margin == value)
                    return;
                _Margin = value;
                _Margin.ParentChanged = BoundingChangedDelegate;
                BoundingChanged();
            }
        }

        internal BoxModelRect _Padding;
        public BoxModelRect Padding
        {
            get => _Padding;
            set
            {
                if (_Padding == value)
                    return;
                _Padding = value;
                _Padding.ParentChanged = BoundingChangedDelegate;
                BoundingChanged();
            }
        }

        internal BoxModelRect _BorderWidth;
        public BoxModelRect BorderWidth
        {
            get => _BorderWidth;
            set
            {
                if (_BorderWidth == value)
                    return;
                _BorderWidth = value.Clone();
                _BorderWidth.ParentChanged = BoundingChangedDelegate;
                BoundingChanged();
            }
        }

        internal BoxModelRect _BorderColor;
        public BoxModelRect BorderColor
        {
            get => _BorderColor;
            set
            {
                if (_BorderColor == value)
                    return;
                _BorderColor = value;
            }
        }

        internal BoxModelRect _BorderStyle;
        public BoxModelRect BorderStyle
        {
            get => _BorderColor;
            set
            {
                if (_BorderColor == value)
                    return;
                _BorderColor = value;
            }
        }

        internal StyleValue _BorderRadius;
        public StyleValue BorderRadius
        {
            get => _BorderRadius;
            set
            {
                if (_BorderRadius == value)
                    return;
                _BorderRadius = value;
            }
        }

    }

}
