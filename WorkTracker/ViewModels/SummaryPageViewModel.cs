using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Navigation;
using Telerik.XamarinForms.Input;
using WorkTracker.Classes;
using WorkTracker.Contracts;
using WorkTracker.Database.DTOs;
using Xamarin.Essentials;

namespace WorkTracker.ViewModels
{
    public class SummaryPageViewModel : ViewModelBase
    {
        private readonly IAssignmentDataAccessService _assignmentDAService;
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
        private ObservableCollection<string> _previousCommentsList;
        private DelegateCommand<object> _selectedDateChangedCommand;

        public SummaryPageViewModel(INavigationService navigationService, INotificationService notify,
            IAssignmentDataAccessService assignmentDataAccessService) : base(
            navigationService)
        {
            _notificationService = notify;
            _assignmentDAService = assignmentDataAccessService;
            var date = DateTime.Today;
            _currentAssignment = new AssignmentDTO();
        }

        private int _ownerId => Preferences.Get(Constants.UserId, 0);

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

        public string UserComment
        {
            get => _comment;
            set => SetProperty(ref _comment, value);
        }

        public ObservableCollection<Appointment> Appointments
        {
            get => _appointments;
            set => SetProperty(ref _appointments, value);
        }

        public DelegateCommand<object> SelectedDateChangedCommand =>
            _selectedDateChangedCommand ??= new DelegateCommand<object>(ExecuteSelectedDateChangedCommand);

        public DelegateCommand<object> AddCommentsCommand =>
            _addCommentsCommand ??= new DelegateCommand<object>(ExecuteAddCommentsCommand);

        public bool CanAddComments
        {
            get => _canAddComments;
            set => SetProperty(ref _canAddComments, value);
        }

        private async void ExecuteAddCommentsCommand(object obj)
        {
            try
            {
                var comment = await _assignmentDAService.AddComment(_currentAssignment.Id, UserComment);
                _currentAssignment.Comments.Add(comment);
                PreviousCommentsList =
                    new ObservableCollection<string>(_currentAssignment.Comments?.Select(x => x.OwnerComment));
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
                _currentDateTime = dateList.FirstOrDefault();
                _currentAssignment = await GetAssignmentForDate(_currentDateTime, _currentWorker);

                if (_currentAssignment == null)
                {
                    ResetUIElemnts();
                    return;
                }

                AssignedJobsList =
                    new ObservableCollection<string>(_currentAssignment.Jobs.Select(x => x.Name));
                CanAddComments = true;
                PreviousCommentsList =
                    new ObservableCollection<string>(_currentAssignment?.Comments?.Select(x => x.OwnerComment));
            }
        }

        private void ResetUIElemnts()
        {
            PreviousCommentsList?.Clear();
            CanAddComments = false;
        }


        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            var workerObj = parameters["Worker"] as WorkerDTO;
            if (workerObj == null)
                return;

            Title = "Summary Of " + workerObj.Name;
            _currentWorker = workerObj;
            _currentDateTime = DateTime.Today;

            try
            {
                Appointments = await GetAppointmentsForMonthYear(DateTime.Today, _currentWorker);

                _currentAssignment = await GetAssignmentForDate(_currentDateTime, _currentWorker);

                if (_currentAssignment != null)
                {
                    CanAddComments = true;
                    AssignedJobsList =
                        new ObservableCollection<string>(_currentAssignment.Jobs.Select(x => x.Name));

                    PreviousCommentsList =
                        new ObservableCollection<string>(_currentAssignment.Comments?.Select(x => x.OwnerComment));
                }
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
        /// Fetches the assignment from server and populate calender with assignment
        /// </summary>
        /// <param name="date"></param>
        /// <param name="workerObj"></param>
        /// <returns></returns>
        private async Task<ObservableCollection<Appointment>> GetAppointmentsForMonthYear(DateTime date,
            WorkerDTO workerObj)
        {
            var startDate = new DateTime(date.Year, date.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var assignmentsThisMonth =
                await _assignmentDAService.GetAllAssignment(_ownerId, startDate, endDate, workerObj.Id);

            var appointments = new ObservableCollection<Appointment>();
            if (assignmentsThisMonth != null)
                foreach (var assignment in assignmentsThisMonth)
                    appointments.Add(
                        new Appointment
                        {
                            StartDate = assignment.AssignedDate,
                            EndDate = assignment.AssignedDate.AddHours(12),
                            Title = assignment.Wage.HasValue ? assignment.Wage.ToString() : string.Empty,
                            Color = Color.Tomato
                        }
                    );
            return new ObservableCollection<Appointment>(appointments);
        }
    }
}