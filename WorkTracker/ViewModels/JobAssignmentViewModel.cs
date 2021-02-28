using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Navigation;
using WorkTracker.Classes;
using WorkTracker.Contracts;
using WorkTracker.Database.DTOs;
using Xamarin.Essentials;

namespace WorkTracker.ViewModels
{
    public class JobAssignmentViewModel : ViewModelBase, IDisposable
    {
        private ObservableCollection<UiBindableJob> _allJobs;
        private DelegateCommand<object> _itemTappedCommand;
        private IJobDataAccessService _jobDAService;
        private DelegateCommand _doneCommand;

        private WorkerDTO _worker;

        public WorkerDTO Worker
        {
            get { return _worker; }
            set { SetProperty(ref _worker, value); }
        }


        public ObservableCollection<UiBindableJob> AllJobs
        {
            get => _allJobs;
            set => SetProperty(ref _allJobs, value);
        }

        public JobAssignmentViewModel(INavigationService navigationService, IJobDataAccessService da) : base(
            navigationService)
        {
            _jobDAService = da;
        }


        public DelegateCommand<object> ItemTappedCommand =>
            _itemTappedCommand ??= new DelegateCommand<object>(ExecuteItemTappedCommand);

        public DelegateCommand DoneCommand =>
            _doneCommand ??= new DelegateCommand(ExecuteDoneCommand);

        void ExecuteDoneCommand()
        {
            var navParam = new NavigationParameters();
            navParam.Add("Jobs", AllJobs.Where(x => x.IsSelected).Select(x => x.Job).ToList());
            navParam.Add("from", Constants.JobAssignmentPage);
            AllJobs.Clear();
            NavigationService.GoBackAsync(navParam);
        }

        private void ExecuteItemTappedCommand(object obj)
        {
            if (obj is UiBindableJob item)
            {
                item.IsSelected = !item.IsSelected;
            }
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters["Worker"] is WorkerDTO worker)
            {
                Worker = worker;
            }


            parameters.TryGetValue("Jobs", out List<JobDTO> alreadyassgnedjobs);

            var result = await _jobDAService.GetAllJob(Preferences.Get(Constants.UserId, 0));

            var bindableResult = result.Select(x => new UiBindableJob()
            {
                Job = x,
                IsSelected = alreadyassgnedjobs?.Any(o => o.Id == x.Id) ?? false
            });

            AllJobs = new ObservableCollection<UiBindableJob>(bindableResult);
        }

        public void Dispose()
        {
            AllJobs.Clear();
        }
    }
}