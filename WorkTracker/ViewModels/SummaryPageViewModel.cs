﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;
using Telerik.XamarinForms.Input;
using WorkTracker.Classes;
using WorkTracker.Contracts;
using WorkTracker.Database.DTOs;
using WorkTracker.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WorkTracker.ViewModels
{
    public class SummaryPageViewModel : ViewModelBase
    {
        private readonly IAssignmentDataAccessService _assignmentDAService;
        private readonly ICachedDataService _cachedDataService;
        private readonly INotificationService _notificationService;
        private DelegateCommand<object> _addCommentsCommand;
        private ObservableCollection<Appointment> _appointments;
        private string _appointmentToDisplay;
        private ObservableCollection<string> _assignedJobsList;
        private bool _canAddComments;
        private string _comment;
        private AssignmentDTO _currentAssignment;
        private DateTime _currentDateTime;
        private WorkerDTO _currentWorker;
        private DelegateCommand<object> _displayDateChangedCommand;

        private DelegateCommand<object> _navigationCommand;
        private ObservableCollection<string> _previousCommentsList;
        private DelegateCommand<object> _selectedDateChangedCommand;
        private DelegateCommand<object> _activatetabCommand;
        private bool _isJobsTabActive;
        private bool _isEmptyData;

        public AssignmentDTO CurrentAssignment
        {
            get => _currentAssignment;
            set
            {
                SetProperty(ref _currentAssignment, value);
            }
        }

        public bool IsEmptyData
        {
            get { return _isEmptyData; }
            set { SetProperty(ref _isEmptyData, value); }
        }

        public SummaryPageViewModel(INavigationService navigationService, INotificationService notify,
            IAssignmentDataAccessService assignmentDataAccessService, IPopupService popupService,ICachedDataService cachedDataService) : base(
            navigationService)
        {
            _popupService = popupService;
            _notificationService = notify;
            _assignmentDAService = assignmentDataAccessService;
            _cachedDataService = cachedDataService;
            var date = DateTime.Today;
            IsJobsTabActive = true;
            CurrentAssignment = new AssignmentDTO();
        }

        public IPopupService _popupService { get; }

        public DelegateCommand<object> NavigationCommand =>
            _navigationCommand ??= new DelegateCommand<object>(ExecuteNavigationCommand);

        private int _ownerId;

        public ObservableCollection<string> PreviousCommentsList
        {
            get => _previousCommentsList;
            set => SetProperty(ref _previousCommentsList, value);
        }

        public ObservableCollection<string> AssignedJobsList
        {
            get => _assignedJobsList;
            set => SetProperty(ref _assignedJobsList, value);
        }

        public DateTime CurrentDate
        {
            get => _currentDateTime;
            set => SetProperty(ref _currentDateTime, value);
        }

        public string UserComment
        {
            get => _comment;
            set => SetProperty(ref _comment, value);
        }
        public bool IsJobsTabActive
        {
            get => _isJobsTabActive;
            set => SetProperty(ref _isJobsTabActive, value);
        }
        public ObservableCollection<Appointment> Appointments
        {
            get => _appointments;
            set => SetProperty(ref _appointments, value);
        }

        public DelegateCommand<object> SelectedDateChangedCommand =>
            _selectedDateChangedCommand ??= new DelegateCommand<object>(ExecuteSelectedDateChangedCommand);

        public DelegateCommand<object> DisplayDateChangedCommand =>
            _displayDateChangedCommand ??= new DelegateCommand<object>(ExecuteDisplayDateChangedCommand);

        public DelegateCommand<object> AddCommentsCommand =>
            _addCommentsCommand ??= new DelegateCommand<object>(ExecuteAddCommentsCommand);

        public bool CanAddComments
        {
            get => _canAddComments;
            set => SetProperty(ref _canAddComments, value);
        }

        private void ExecuteNavigationCommand(object obj)
        {
            string pageAlias = obj.ToString();

            NavigationService.NavigateAsync(pageAlias, new NavigationParameters()
            {
                {"worker",_currentWorker}
            });
        }

        private async void ExecuteDisplayDateChangedCommand(object obj)
        {
            var date = (DateTime)obj;
            _popupService.ShowLoadingScreen();
            try
            {
                Appointments = await GetAppointmentsForMonthYear(date, _currentWorker);
                SelectedDateChangedCommand.Execute(new List<DateTime> { date });
            }
            catch
            {

            }
            finally
            {
                _popupService.HideLoadingScreen();
            }
        }

        private async void ExecuteAddCommentsCommand(object obj)
        {
            try
            {
                var comment = await _assignmentDAService.AddComment(CurrentAssignment.Id, UserComment);
                CurrentAssignment.Comments.Add(comment);
                PreviousCommentsList.Clear();
                PreviousCommentsList =
                    new ObservableCollection<string>(CurrentAssignment.Comments?.Select(x => x.OwnerComment));
                UserComment = string.Empty;
                CheckEmptyData();
            }
            catch (Exception e)
            {
                _notificationService.Notify(e.Message, NotificationTypeEnum.Error);
            }
        }

        private async void ExecuteSelectedDateChangedCommand(object ob)
        {
            if (ob is IList<DateTime> dateList)
            {
                CurrentDate = dateList.FirstOrDefault();
                CurrentAssignment = await GetAssignmentForDate(CurrentDate, _currentWorker);

                if (CurrentAssignment == null)
                {
                    ResetUIElemnts();
                    CheckEmptyData();
                    return;
                }
                AssignedJobsList?.Clear();
                AssignedJobsList =
                    new ObservableCollection<string>(CurrentAssignment.Jobs.Select(x => x.Name));
                CanAddComments = true;
                PreviousCommentsList?.Clear();
                PreviousCommentsList =
                    new ObservableCollection<string>(CurrentAssignment?.Comments?.Select(x => x.OwnerComment));

                CheckEmptyData();
            }
        }

        private void CheckEmptyData()
        {
            IsEmptyData = IsJobsTabActive switch
            {
                true => AssignedJobsList == null || AssignedJobsList.Count == 0,
                false => PreviousCommentsList == null || PreviousCommentsList.Count == 0
            };
        }

        private void ResetUIElemnts()
        {
            PreviousCommentsList?.Clear();
            AssignedJobsList?.Clear();
            CanAddComments = false;
        }

        public ICommand ActivatetabCommand =>
            _activatetabCommand ??= new DelegateCommand<object>(obj =>
            {
                var tab = (JobCommentEnum)obj;
                IsJobsTabActive = tab switch
                {
                    JobCommentEnum.Comments => false,
                    JobCommentEnum.Jobs => true,
                    _ => IsJobsTabActive
                };

                CheckEmptyData();
            });

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            _ownerId = _cachedDataService.GetCachedOwner().Id;

            var workerObj = parameters["Worker"] as WorkerDTO;
            if (workerObj == null)
                return;

            Title = "Summary Of " + workerObj.Name;
            _currentWorker = workerObj;
            CurrentDate = DateTime.Today;

            try
            {
                Appointments = await GetAppointmentsForMonthYear(DateTime.Today, _currentWorker);

                CurrentAssignment = await GetAssignmentForDate(CurrentDate, _currentWorker);

                if (CurrentAssignment != null)
                {
                    CanAddComments = true;

                    AssignedJobsList =
                        new ObservableCollection<string>(CurrentAssignment.Jobs.Select(x => x.Name));

                    PreviousCommentsList =
                        new ObservableCollection<string>(CurrentAssignment.Comments?.Select(x => x.OwnerComment));


                }
                CheckEmptyData();
            }
            catch (Exception e)
            {
                _notificationService.Notify(e.Message, NotificationTypeEnum.Error);
            }
        }

        private async Task<AssignmentDTO> GetAssignmentForDate(DateTime date, WorkerDTO workerObj)
        {
            try
            {
                var result = await _assignmentDAService.GetAllAssignment(_ownerId, date, date, workerObj.Id);
                return result.FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        ///     Fetches the assignment from server and populate calender with assignment
        /// </summary>
        /// <param name="date"></param>
        /// <param name="workerObj"></param>
        /// <returns></returns>
        private async Task<ObservableCollection<Appointment>> GetAppointmentsForMonthYear(DateTime date,
            WorkerDTO workerObj)
        {
            _popupService.ShowLoadingScreen();
            var appointments = new ObservableCollection<Appointment>();
            try
            {
                var startDate = new DateTime(date.Year, date.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var assignmentsThisMonth =
                    await _assignmentDAService.GetAllAssignment(_ownerId, startDate, endDate, workerObj.Id);


                if (assignmentsThisMonth != null)
                    foreach (var assignment in assignmentsThisMonth)
                        appointments.Add(
                            new Appointment
                            {
                                StartDate = assignment.AssignedDate,
                                EndDate = assignment.AssignedDate.AddHours(12),
                                Title = string.Empty,
                                Color = (Xamarin.Forms.Color)Application.Current.Resources["SubmitCoinFillColor"]
                            }
                        );
            }
            catch (Exception e)
            {
                _notificationService.Notify(e.InnerException != null ? e.InnerException.Message : e.Message,
                    NotificationTypeEnum.Error);
            }
            finally
            {
                _popupService.HideLoadingScreen();
            }

            return new ObservableCollection<Appointment>(appointments);
        }
    }
}