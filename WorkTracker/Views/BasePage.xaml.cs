using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BasePage : ContentPage
    {
        
        // BindableProperty implementation
        public static readonly BindableProperty BackButtonPressedCommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(BasePage), null);

        public ICommand BackButtonPressedCommand
        {
            get { return (ICommand)GetValue(BackButtonPressedCommandProperty); }
            set { SetValue(BackButtonPressedCommandProperty, value); }
        }

        public BasePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// On back button pressed
        /// </summary>
        /// <returns></returns>
        protected override bool OnBackButtonPressed()
        {
            return BackButtonPressedCommand != null ? ExecuteBackButtonPressedCommand() : base.OnBackButtonPressed(); // if back button pressed command is set, execute back button command else execute default action
        }

        /// <summary>
        /// Raise Back button Pressed command
        /// </summary>
        /// <returns>True if handled otherwise false</returns>
        private bool ExecuteBackButtonPressedCommand()
        {
            if (BackButtonPressedCommand == null) return false; // check if command is set

            var backButtonPressedEventArgs = new BackButtonPressedEventArgs(); // get new event argument for back button pressed

            if (!BackButtonPressedCommand.CanExecute(backButtonPressedEventArgs)) return false; // check if tt can be executed

            BackButtonPressedCommand.Execute(backButtonPressedEventArgs); // execute command

            return backButtonPressedEventArgs.Handled; // return whether handled or not

        }
    }
}