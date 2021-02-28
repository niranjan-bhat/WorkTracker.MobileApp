using System;
using System.Collections.Generic;
using System.Text;
using Telerik.XamarinForms.Input;
using Xamarin.Forms;

namespace WorkTracker.CustomControls
{
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
            Color.White);

        public CustomEntry() : base()
        {
           
        }
    }
}
