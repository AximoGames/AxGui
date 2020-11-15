// This file is part of AxGUI. Web: https://github.com/AximoGames
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

        internal bool _BoundingChanged;
        private void BoundingChanged()
        {
            _BoundingChanged = true;
        }

        private Action BoundingChangedDelegate;

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
                _Anchors = value.Clone();
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
                _Margin = value.Clone();
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
                _Padding = value.Clone();
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
                _BorderColor = value.Clone();
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
                _BorderColor = value.Clone();
            }
        }

        internal BoxModelRect _BorderRadius;
        public BoxModelRect BorderRadius
        {
            get => _BorderRadius;
            set
            {
                if (_BorderRadius == value)
                    return;
                _BorderRadius = value.Clone();
            }
        }

    }

}
