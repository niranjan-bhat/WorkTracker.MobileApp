using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Navigation;

namespace WorkTracker.ViewModels
{
    public class dummyViewModel : ViewModelBase
    {
        private DelegateCommand _fieldName;
        public DelegateCommand CommandName =>
            _fieldName ?? (_fieldName = new DelegateCommand(ExecuteCommandName));

        private ObservableCollection<string> _contryList;
        public ObservableCollection<string> ContryList
        {
            get { return _contryList; }
            set { SetProperty(ref _contryList, value); }
        }
        void ExecuteCommandName()
        {
            Random r = new Random();
            
            List<string> a = new List<string>();
            for (int i = 0; i < r.Next(20); i++)
            {
                a.Add(r.Next(100).ToString());
            }
            ContryList = new ObservableCollection<string>(a);
        }
        public dummyViewModel(INavigationService na) : base(na)
        {

        }
    }
}
