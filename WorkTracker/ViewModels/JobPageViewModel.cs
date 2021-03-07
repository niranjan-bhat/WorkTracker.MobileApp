using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using WorkTracker.Classes;
using WorkTracker.Contracts;
using WorkTracker.Database.DTOs;
using WorkTracker.Events;
using Xamarin.Essentials;

namespace WorkTracker.ViewModels
{
    public class JobViewModel : BindableBase, IDisposable
    {
        private readonly IEventAggregator eventAggregator;
        private ICommand _addJobCommand;
        private ObservableCollection<JobDTO> _allJobs;

        private bool _isNoJob;
        private JobDTO _Job;
        private readonly IJobDataAccessService _jobDAService;
        private INotificationService _notificationService;

        public JobViewModel(IJobDataAccessService jobDataAccessService, IEventAggregator ea, INotificationService notificationService)
        {
            _jobDAService = jobDataAccessService;
            eventAggregator = ea;
            _notificationService = notificationService;
            Job = new JobDTO();
            GetAllJobs();
            SubscribeToEvents();
        }

        private int _ownerId => Preferences.Get(Constants.UserId, 0);

        public bool IsNoJob
        {
            get => _isNoJob;
            set => SetProperty(ref _isNoJob, value);
        }

        public ObservableCollection<JobDTO> AllJobs
        {
            get => _allJobs;
            set => SetProperty(ref _allJobs, value);
        }

        public JobDTO Job
        {
            get => _Job;
            set => SetProperty(ref _Job, value);
        }

        public ICommand AddJobCommand =>
            _addJobCommand ??= new DelegateCommand(ExecuteAddJobCommand);

        private void SubscribeToEvents()
        {
            eventAggregator.GetEvent<JobAddedEvent>().Subscribe(EventHandler);
        }

        private void EventHandler(JobDTO obj)
        {
            AllJobs.Insert(0, obj);
            RaisePropertyChanged(nameof(AllJobs));
        }

        private async void ExecuteAddJobCommand()
        {
            if (string.IsNullOrEmpty(Job.Name))
                return;

            try
            {
                await _jobDAService.InsertJob(_ownerId, Job.Name);
                IsNoJob = false;
                eventAggregator.GetEvent<JobAddedEvent>().Publish(new JobDTO { Name = Job.Name });
                _notificationService.Notify(Resource.JobAddedSuccess, NotificationTypeEnum.Success);
            }
            catch (Exception e)
            {
                _notificationService.Notify(e.InnerException == null ?e.Message : e.InnerException.Message, NotificationTypeEnum.Error);
            }
        }


        private async void GetAllJobs()
        {
            var result = await _jobDAService.GetAllJob(_ownerId);
            IsNoJob = result?.Count == 0;
            if (result != null) AllJobs = new ObservableCollection<JobDTO>(result);
        }

        public void Dispose()
        {
            AllJobs.Clear();
        }
    }
}