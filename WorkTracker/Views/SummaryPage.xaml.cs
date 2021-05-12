using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.XamarinForms.Common;
using Telerik.XamarinForms.Input.Calendar;
using Telerik.XamarinForms.Primitives;
using WorkTracker.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SummaryPage : ContentPage
    {
        public SummaryPage()
        {
            InitializeComponent();
        }

        private void ClosePopup(object sender, EventArgs e)
        {
            popup.IsOpen = false;
            if ((this.BindingContext) is SummaryPageViewModel context)
            {
                context.AddCommentsCommand.Execute(null);
            }
        }

        private void ShowPopup(object sender, EventArgs e)
        {
            popup.PlacementTarget = MainGrid;
            popup.Placement = PlacementMode.Center;
            popup.IsOpen = true;

        }
    }
}