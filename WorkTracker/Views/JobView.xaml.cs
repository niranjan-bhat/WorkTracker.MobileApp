using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Ioc;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JobView : ContentView
    {
        public JobView()
        {
            InitializeComponent();
        }
    }
}