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

        internal bool _BoundingChanged = false;
        private void BoundingChanged()
        {
            _BoundingChanged = true;
        }

        private Action BoundingChangedDelegate;

        public StylePosition Position;
        public StyleDisplay Display;
        public StyleVisibility Visibility;

        internal StyleValue _Width;
        public StyleValue Width
        {
            get => _Width;
            set
            {
                if (_Width == value)
                    return;
                _Width = value;
                BoundingChanged();
            }
        }

        internal StyleValue _Height;
        public StyleValue Height
        {
            get => _Height;
            set
            {
                if (_Height == value)
                    return;
                _Height = value;
                BoundingChanged();
            }
        }

        internal BoxModelRect _Anchors = new BoxModelRect();
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

        internal BoxModelRect _Margin = new BoxModelRect();
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

        internal BoxModelRect _Padding = new BoxModelRect();
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

        internal BoxModelRect _BorderWidth = new BoxModelRect();
        public BoxModelRect BorderWidth
        {
            get => _BorderWidth;
            set
            {
                if (_BorderWidth == value)
                    return;
                _BorderWidth = value;
                _BorderWidth.ParentChanged = BoundingChangedDelegate;
                BoundingChanged();
            }
        }

        internal BoxModelRect _BorderColor = new BoxModelRect();
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

        internal BoxModelRect _BorderStyleWidth = new BoxModelRect();
        public BoxModelRect BorderStyleWidth
        {
            get => _BorderColor;
            set
            {
                if (_BorderColor == value)
                    return;
                _BorderColor = value;
            }
        }

        internal BoxModelRect _BorderRadiusWidth = new BoxModelRect();
        public BoxModelRect BorderRadiusWidth
        {
            get => _BorderRadiusWidth;
            set
            {
                if (_BorderRadiusWidth == value)
                    return;
                _BorderRadiusWidth = value;
            }
        }

    }

}
