using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkTracker.CustomControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListViewPopUpButton : ContentView
    {
        public ListViewPopUpButton()
        {
            InitializeComponent();
            this.popup.PlacementTarget = null;
        }

        public IEnumerable<string> Items
        {
            get => (IEnumerable<string>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public static readonly BindableProperty ItemsProperty = BindableProperty.Create(
            nameof(Items),
            typeof(IEnumerable<string>),
            typeof(ListViewPopUpButton), propertyChanged: ExecuteListChanged
        );

        private static void ExecuteListChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = bindable as ListViewPopUpButton;
            if (control != null && newValue is IEnumerable<string> list)
            {
                control.MainListView.ItemsSource = list;
            }
        }

        public static readonly BindableProperty PopupHeadingTextProperty = BindableProperty.Create(
            propertyName: nameof(PopupHeadingText),
            returnType: typeof(string),
            declaringType: typeof(ListViewPopUpButton),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: PopupHeadingPropertyChanged);

        public string PopupHeadingText
        {
            get => (string)GetValue(PopupHeadingTextProperty);
            set => SetValue(PopupHeadingTextProperty, value);
        }

        private static void PopupHeadingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ListViewPopUpButton)bindable;
            control.PopUpHeadingLabel.Text = newValue.ToString();
        }
        private void MainButton_OnClicked(object sender, EventArgs e)
        {
            this.popup.IsOpen = !this.popup.IsOpen;
        }
    }
}