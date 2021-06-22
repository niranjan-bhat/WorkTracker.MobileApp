using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using Prism.Navigation;
using WorkTracker.Classes;
using WorkTracker.Contracts;
using WorkTracker.Database.DTOs;
using WorkTracker.Services;
using Xamarin.Essentials;

namespace WorkTracker.ViewModels
{
    public class JobAssignmentViewModel : ViewModelBase, IDisposable
    {
        private ObservableCollection<UiBindableJob> _allJobs;
        private DelegateCommand _doneCommand;
        private DelegateCommand<object> _itemTappedCommand;
        private readonly INotificationService _notificationService;
        private readonly IPopupService _popupService;
        private readonly IJobDataAccessService _jobDAService;
        private readonly ICachedDataService _cachedDataService;

        private WorkerDTO _worker;
        private bool _isNoJob;

        public JobAssignmentViewModel(INavigationService navigationService, 
            INotificationService notificationService, IPopupService popupService, IJobDataAccessService da, ICachedDataService cachedDataService) : base(
            navigationService)
        {
            _notificationService = notificationService;
            _popupService = popupService;
            _jobDAService = da;
            _cachedDataService = cachedDataService;
        }

        public bool IsNoJob
        {
            get => _isNoJob;
            set => SetProperty(ref _isNoJob, value);
        }
        public WorkerDTO Worker
        {
            get => _worker;
            set => SetProperty(ref _worker, value);
        }


        public ObservableCollection<UiBindableJob> AllJobs
        {
            get => _allJobs;
            set => SetProperty(ref _allJobs, value);
        }


        public DelegateCommand<object> ItemTappedCommand =>
            _itemTappedCommand ??= new DelegateCommand<object>(ExecuteItemTappedCommand);

        public DelegateCommand DoneCommand =>
            _doneCommand ??= new DelegateCommand(ExecuteDoneCommand);

        public void Dispose()
        {
            AllJobs.Clear();
        }

        private void ExecuteDoneCommand()
        {
            try
            {
                var navParam = new NavigationParameters();
                navParam.Add("Jobs", AllJobs.Where(x => x.IsSelected).Select(x => x.Job).ToList());
                navParam.Add("from", Constants.JobAssignmentPage);
                AllJobs.Clear();
                NavigationService.GoBackAsync(navParam);
            }
            catch (Exception e)
            {
                _notificationService.Notify(Resource.Failure,NotificationTypeEnum.Error);
            }
        }

        private void ExecuteItemTappedCommand(object obj)
        {
            if (obj is UiBindableJob item) item.IsSelected = !item.IsSelected;
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters["Worker"] is WorkerDTO worker) Worker = worker;

            _popupService.ShowLoadingScreen();


            try
            {
                parameters.TryGetValue("JobsAssigned", out List<JobDTO> alreadyassgnedjobs);
                parameters.TryGetValue("AllJobs", out List<JobDTO> allJobs);

                var bindableResult = allJobs?.Select(x => new UiBindableJob
                {
                    Job = x,
                    IsSelected = alreadyassgnedjobs?.Any(o => o.Id == x.Id) ?? false
                });
                AllJobs = new ObservableCollection<UiBindableJob>(bindableResult);
                IsNoJob = AllJobs.Count == 0;
            }
            catch (Exception e)
            {

                _notificationService.Notify(e.Message, NotificationTypeEnum.Error);
            }
            finally
            {
                _popupService.HideLoadingScreen();
            }
        }
    }
}