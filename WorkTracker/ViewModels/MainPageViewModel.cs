using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using WorkTracker.Classes;
using WorkTracker.Contracts;
using WorkTracker.Database.DTOs;
using WorkTracker.Events;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WorkTracker.ViewModels
{
    public class MainPageViewModel : ViewModelBase, IDisposable
    {
        public delegate void AttendanceDateChanged();

        private readonly IAssignmentDataAccessService _assignmentDataAccessService;
        private readonly IEventAggregator _eventAggrigator;
        private readonly IJobDataAccessService _jobDataAccessService;
        private readonly INotificationService _notify;
        private readonly IPopupService _popupService;
        private readonly IWorkerDataAccessService _workerDataAccessService;
        private DelegateCommand _addNewWorkerCommand;
        private ObservableCollection<UiBindableAssignment> _allAssignments;
        private ObservableCollection<WorkerDTO> _allWorkers;
        private DelegateCommand<BackButtonPressedEventArgs> _backButtonPressCommand;
        private bool _isAssignmentSubmittedAlreadies;
        private bool _isNoWorker;
        private DelegateCommand<object> _navigateToJobAssignmentCommand;
        private DelegateCommand<object> _navigateToSummaryCommand;
        private WorkerDTO _selectedWorker;
        private DelegateCommand _submitAttendance;
        private DateTime? _userSelectedDate = DateTime.Today;

        public MainPageViewModel(INavigationService navigationService, IPopupService popupService,
            IEventAggregator ea, INotificationService ns, IAssignmentDataAccessService assignmentDataAccessService,
            IWorkerDataAccessService workerDataAccessService, IJobDataAccessService jobDataAccessService)
            : base(navigationService)
        {
            Title = "WorkTrack";
            _eventAggrigator = ea;
            _notify = ns;
            _workerDataAccessService = workerDataAccessService;
            _assignmentDataAccessService = assignmentDataAccessService;
            _jobDataAccessService = jobDataAccessService;
            _popupService = popupService;
        }

        public bool IsAssignmentSubmittedAlready
        {
            get => _isAssignmentSubmittedAlreadies;
            set => SetProperty(ref _isAssignmentSubmittedAlreadies, value);
        }

        public bool IsNoWorker
        {
            get => _isNoWorker;
            set => SetProperty(ref _isNoWorker, value);
        }

        public DelegateCommand<BackButtonPressedEventArgs> BackButtonPressCommand =>
            _backButtonPressCommand ??= new DelegateCommand<BackButtonPressedEventArgs>(ExecuteBackButtonPressCommand);

        /// <summary>
        /// Minimum date to choose to mark the attendance.
        /// </summary>
        public DateTime MinimumSelectableDate => DateTime.Now.AddDays(-100);

        public ObservableCollection<WorkerDTO> AllWorkers
        {
            get => _allWorkers;
            set => SetProperty(ref _allWorkers, value);
        }

        public DateTime? SelectedDate
        {
            get => _userSelectedDate;
            set
            {
                SetProperty(ref _userSelectedDate, value);
                DateSelectionModified?.Invoke();
            }
        }

        public ObservableCollection<UiBindableAssignment> AllAssignments
        {
            get => _allAssignments;
            set => SetProperty(ref _allAssignments, value);
        }

        public DelegateCommand AddNewWorkerCommand =>
            _addNewWorkerCommand ??= new DelegateCommand(ExecuteAddNewWorkerCommand);

        public DelegateCommand SubmitAttendanceCommand =>
            _submitAttendance ??= new DelegateCommand(ExecuteSubmitAttendanceCommand);

        public DelegateCommand<object> NavigateToSummaryCommand =>
            _navigateToSummaryCommand ??= new DelegateCommand<object>(ExecuteNavigateToSummaryCommand);

        public DelegateCommand<object> NavigateToJobAssignmentCommand =>
            _navigateToJobAssignmentCommand ??= new DelegateCommand<object>(ExecuteNavigateToJobAssignmentCommand);

        public void Dispose()
        {
            AllWorkers.Clear();
            AllAssignments.Clear();
        }

        /// <summary>
        /// Event to be raised when user changes the date to mark attendance.
        /// </summary>
        public event AttendanceDateChanged DateSelectionModified;

        /// <summary>
        /// Gets all the attendance marked for the date chosen
        /// </summary>
        private async void FetchSubmittedAttendanceForSelectedDate()
        {
            var ownerId = Preferences.Get(Constants.UserId, 0);
            _popupService.ShowLoadingScreen();
            try
            {
                var submittedAssignment =
                    await _assignmentDataAccessService.GetAllAssignment(ownerId, SelectedDate.Value, SelectedDate.Value,
                        null);

                if (!submittedAssignment.Any()) // If there is no attendance submitted for this date
                {
                    foreach (var assignment in AllAssignments) assignment.IsAttendanceSubmitted = false; //For all the pre-existing assignment on the UI, mark as not-submitted
                    IsAssignmentSubmittedAlready = false; //Mark assignment is not submitted to this date
                    return;
                }

                //If assignment is fetched for this date

                IsAssignmentSubmittedAlready = true; //Mark assignment is already submitted
                foreach (var assignmentToDeSelect in AllAssignments) //For all the worker present in UI, clear the data. Data is added in next step
                {
                    assignmentToDeSelect.IsSelected = false;
                    assignmentToDeSelect.IsAttendanceSubmitted = false;
                    assignmentToDeSelect.AssignedJobs?.Clear();
                    assignmentToDeSelect.Wage = null;
                }

                foreach (var assignment in submittedAssignment
                ) //For all the assignment present in this data, update the UI
                {
                    var assignmentForWorker =
                        AllAssignments.FirstOrDefault(x => x.Assignment.Worker.Id == assignment.WorkerId);
                    if (assignmentForWorker != null)
                    {

                        assignmentForWorker.Wage = assignment.Wage;
                        assignmentForWorker.IsAttendanceSubmitted = true;
                        assignmentForWorker.AssignedJobs = new ObservableCollection<JobDTO>(assignment.Jobs);
                        assignmentForWorker.IsSelected = true;
                        assignmentForWorker.Assignment.Id = assignment.Id;
                    }
                }
            }
            catch (Exception e)
            {
                _notify.Notify(e.Message, NotificationTypeEnum.Error);
            }
            finally
            {
                _popupService.HideLoadingScreen();
            }
        }

        /// <summary>
        /// Log out logic
        /// </summary>
        /// <param name="arg"></param>
        private async void ExecuteBackButtonPressCommand(BackButtonPressedEventArgs arg)
        {
            arg.Handled = true;
            if (await _popupService.ShowPopup("Are you sure?", "Do you want to log out?", "Yes", "No"))
                await NavigationService.GoBackAsync();
        }


        private void ExecuteNavigateToJobAssignmentCommand(object obj)
        {
            var assignment = (UiBindableAssignment)obj;
            _selectedWorker = assignment.Assignment.Worker;
            NavigationService.NavigateAsync(Constants.JobAssignmentPage,
                new NavigationParameters { { "Worker", _selectedWorker }, { "Jobs", assignment.AssignedJobs?.ToList() } });
        }

        private void ExecuteAddNewWorkerCommand()
        {
            NavigationService.NavigateAsync(Constants.AddWorkerPage);
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            string from;
            parameters.TryGetValue("from", out from);

            switch (from)
            {
                case Constants.JobAssignmentPage:
                    var jobs = parameters.GetValue<List<JobDTO>>("Jobs");
                    UpdateAssignmentWithNewlyAddedJobs(jobs);
                    break;
                case Constants.Login:
                    IsAssignmentSubmittedAlready = false;
                    SelectedDate = Preferences.Get(Constants.LatestDateOfAttendanceSubmission, DateTime.Today);
                    AllWorkers = await GetAllWorkers();
                    AllAssignments = PopulateAssignment(AllWorkers);
                    SubscribeEvents();
                    IsNoWorker = AllWorkers.Count == 0;
                    break;
            }
        }

        /// <summary>
        /// On assigning new jobs for a worker, update the assignment locally.
        /// </summary>
        /// <param name="jobs"></param>
        private void UpdateAssignmentWithNewlyAddedJobs(List<JobDTO> jobs)
        {
            var assignment = AllAssignments.FirstOrDefault(x => x.Assignment.Worker.Id == _selectedWorker.Id);
            assignment.AssignedJobs = new ObservableCollection<JobDTO>(jobs);
        }

        private void SubscribeEvents()
        {
            _eventAggrigator.GetEvent<WorkerModifiedEvent>().Subscribe(WorkerModifiedEventHandler);
            DateSelectionModified += FetchSubmittedAttendanceForSelectedDate;
        }

        /// <summary>
        /// Handle worker data modified/added
        /// </summary>
        /// <param name="obj"></param>
        private async void WorkerModifiedEventHandler(WorkerModifiedEventArguments obj)
        {
            switch (obj.ModificationType)
            {
                case CrudEnum.Added:
                    var ownerId = Preferences.Get(Constants.UserId, 0);
                    var workerList = await _workerDataAccessService.GetAllWorker(ownerId);
                    AllWorkers = new ObservableCollection<WorkerDTO>(workerList);
                    IsNoWorker = false;
                    break;
            }

            AllAssignments = PopulateAssignment(AllWorkers);
        }

        private async Task<ObservableCollection<WorkerDTO>> GetAllWorkers()
        {
            var ownerId = Preferences.Get(Constants.UserId, 1);
            try
            {
                var workerList = await _workerDataAccessService.GetAllWorker(ownerId);
                return new ObservableCollection<WorkerDTO>(workerList);
            }
            catch (Exception e)
            {
                _notify.Notify(e.Message, NotificationTypeEnum.Error);
            }

            return new ObservableCollection<WorkerDTO>();
        }

        /// <summary>
        /// Creates the UI bindable assignment objects from data retrieved from server
        /// </summary>
        /// <param name="workers"></param>
        /// <returns></returns>
        private ObservableCollection<UiBindableAssignment> PopulateAssignment(ObservableCollection<WorkerDTO> workers)
        {
            var assignments = new ObservableCollection<UiBindableAssignment>();
            foreach (var worker in workers)
                assignments.Add(new UiBindableAssignment
                {
                    Assignment = new AssignmentDTO
                    {
                        AssignedDate = DateTime.Now,
                        Worker = worker
                    },
                    IsSelected = true
                });
            return assignments;
        }

        private void ExecuteNavigateToSummaryCommand(object param)
        {
            if (param == null)
                return;

            var navPara = new NavigationParameters();
            navPara.Add("Worker", param);
            NavigationService.NavigateAsync(Constants.SummaryPage, navPara);
        }

        /// <summary>
        /// Add a new attendance/ update existing attendance
        /// </summary>
        private async void ExecuteSubmitAttendanceCommand()
        {
            IsBusy = true;
            try
            {
                await ValidateAttendanceForSubmission(); //Validation before adding assignment

                //Give a warning if attendance is submitted already for the chosen date
                if (IsAssignmentSubmittedAlready && !await _popupService.ShowPopup(Resource.Warning,
                    Resource.AttendanceSubmissionWarning, Resource.Yes,
                    Resource.No))
                {
                }
                else
                {
                    var ownerId = Preferences.Get(Constants.UserId, 0);

                    //Delete existing attendance if any.
                    await _assignmentDataAccessService.DeleteAssignments(ownerId, SelectedDate.Value);

                    foreach (var uiBindableAssignment in AllAssignments.Where(x => x.IsSelected)) //Submit the attendance for selected workers only
                    {
                        await _assignmentDataAccessService.InsertAssignment(ownerId,
                            uiBindableAssignment.Wage.HasValue
                                ? uiBindableAssignment.Wage.Value
                                : 0,
                            uiBindableAssignment.Assignment.Worker.Id, SelectedDate.Value,
                            uiBindableAssignment.AssignedJobs?.ToList());

                        uiBindableAssignment.IsAttendanceSubmitted = true;
                    }

                    IsAssignmentSubmittedAlready = true;
                    _notify.Notify("Success", NotificationTypeEnum.Success);
                    Preferences.Set(Constants.LatestDateOfAttendanceSubmission, SelectedDate.Value);
                }
            }
            catch (Exception e)
            {
                _notify.Notify(Resource.Failure, NotificationTypeEnum.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task<bool> ValidateAttendanceForSubmission()
        {
            var assignmentsToSubmit = AllAssignments.Where(x => x.IsSelected);

            if (!assignmentsToSubmit.ToList().Any()) throw new Exception("Unable to submit"); //If assignment is selected for insertion

            var noJobAssignments = assignmentsToSubmit.Where(x => x.AssignedJobs == null);

            var assignments = noJobAssignments as UiBindableAssignment[] ?? noJobAssignments.ToArray();

            if (assignments.Any()) //Warn user for not adding job for assigning 
            {
                var workersName = assignments.Select(x => x.Assignment.Worker.Name);
                var enumerable = workersName as string[] ?? workersName.ToArray();

                throw new Exception(enumerable.Length <= 10
                    ? "Please assign at least one job to " + string.Join(",", enumerable)
                    : "Please assign at least one job to workers");
            }

            return true;
        }
    }
}