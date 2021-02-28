using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Navigation;
using Telerik.Windows.Documents.Flow.Model;
using Telerik.XamarinForms.Input;
using WorkTracker.Classes;
using WorkTracker.Contracts;
using WorkTracker.Database.DTOs;
using Xamarin.Essentials;

namespace WorkTracker.ViewModels
{
    public class SummaryPageViewModel : ViewModelBase
    {
        private DelegateCommand<object> _addCommentsCommand;
        private ObservableCollection<Appointment> _appointments;
        private string _appointmentToDisplay;
        private bool _canAddComments;
        private string _comment;
        private AssignmentDTO _currentAssignment;
        private DateTime _currentDateTime;
        private WorkerDTO _currentWorker;
        private ObservableCollection<string> _previousCommentsList;
        private DelegateCommand<object> _selectedDateChangedCommand;
        private IAssignmentDataAccessService _assignmentDAService;
        private INotificationService _notificationService;
        int _ownerId => Preferences.Get(Constants.UserId, 0);

        public SummaryPageViewModel(INavigationService navigationService, INotificationService notify, IAssignmentDataAccessService assignmentDataAccessService) : base(
            navigationService)
        {
            _notificationService = notify;
            _assignmentDAService = assignmentDataAccessService;
            var date = DateTime.Today;
            _currentAssignment = new AssignmentDTO();
        }

        public ObservableCollection<string> PreviousCommentsList
        {
            get => _previousCommentsList;
            set => SetProperty(ref _previousCommentsList, value);
        }

        public string AppointmentAsString
        {
            get => _appointmentToDisplay;
            set => SetProperty(ref _appointmentToDisplay, value);
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
                PreviousCommentsList = new ObservableCollection<string>(_currentAssignment.Comments?.Select(x => x.OwnerComment));
            }
            catch (Exception e)
            {
                _notificationService.Notify(e.Message,NotificationTypeEnum.Error);
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

                CanAddComments = true;
                AppointmentAsString = ConvertAppointmentToString(_currentAssignment); ;
                PreviousCommentsList = new ObservableCollection<string>(_currentAssignment?.Comments?.Select(x => x.OwnerComment));
            }
        }

        private void ResetUIElemnts()
        {
            AppointmentAsString = string.Empty;
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

                AppointmentAsString = ConvertAppointmentToString(_currentAssignment);

                if (_currentAssignment != null)
                {
                    CanAddComments = true;

                    PreviousCommentsList =
                        new ObservableCollection<string>(_currentAssignment.Comments?.Select(x => x.OwnerComment));
                }
            }
            catch (Exception e)
            {
                _notificationService.Notify(e.Message, NotificationTypeEnum.Error);
            }
        }

        private string ConvertAppointmentToString(AssignmentDTO assignment)
        {
            if (assignment != null)
                return $"{assignment.Wage} /-";
            return string.Empty;
        }

        private async Task<AssignmentDTO> GetAssignmentForDate(DateTime date, WorkerDTO workerObj)
        {
            try
            {
                var result = await _assignmentDAService.GetAssignmentOnDate(workerObj.Id, date, _ownerId);
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private async Task<ObservableCollection<Appointment>> GetAppointmentsForMonthYear(DateTime date, WorkerDTO workerObj)
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
                            Title = assignment.Wage + "/-",
                            Color = Color.Tomato
                        }
                    );
            return new ObservableCollection<Appointment>(appointments);
        }
    }
}