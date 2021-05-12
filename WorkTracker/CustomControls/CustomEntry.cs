﻿using System;
using System.Collections.Generic;
using System.Text;
using Telerik.XamarinForms.Input;
using Xamarin.Forms;

namespace WorkTracker.CustomControls
{
    /// <summary>
    /// Entry with user defined underline color
    /// </summary>
    public class CustomEntry : Entry
    {
        private Color _underlineColor;
        public Color UnderlineColor
        {
            get { return _underlineColor; }
            set
            {
                _underlineColor = value;
                OnPropertyChanged();
            }
        }
        public static readonly BindableProperty UnderlineColorProperty = BindableProperty.Create(
            nameof(UnderlineColor),
            typeof(Color),
            typeof(Color),
            Color.White,
            propertyChanged:SetUnderlineColor);

        private static void SetUnderlineColor(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is CustomEntry control && newvalue is Color newColor)
            {
                control.UnderlineColor = newColor;
            }
        }

        public CustomEntry() : base()
        {
           
        }
    }
}
