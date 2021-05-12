using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace WorkTracker.ViewModels
{
    public class PaymentViewModel : BindableBase
    {
        private bool _isSalaryTabActive;
        public bool IsSalaryTabActive
        {
            get { return _isSalaryTabActive; }
            set { SetProperty(ref _isSalaryTabActive, value); }
        }

        private ICommand _toggleTabActiveCommand;
        public ICommand ToggleTabActiveCommand =>
            _toggleTabActiveCommand ??= new DelegateCommand(ExecuteToggleTabActiveCommand);

        void ExecuteToggleTabActiveCommand()
        {
            IsSalaryTabActive = !IsSalaryTabActive;
        }
        public PaymentViewModel()
        {
            IsSalaryTabActive = true;
        }
    }
}
