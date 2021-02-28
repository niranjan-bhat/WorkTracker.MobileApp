using System;
using System.Collections.Generic;
using System.Text;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace WorkTracker
{
    public class TabWithIcon : TabViewHeaderItem
    {

        public TabWithIcon():base()
        {
        }
        public string TabHeaderImage { get; set; }

        public static readonly BindableProperty TitleTextProperty = BindableProperty.Create(
            propertyName: "TabHeaderImage",
            returnType: typeof(string),
            declaringType: typeof(TabWithIcon),
            defaultValue: "",
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: TitleTextPropertyChanged);
        private static void TitleTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (TabWithIcon)bindable;
            control.TabHeaderImage = (string) newValue;
        }

    }
}
