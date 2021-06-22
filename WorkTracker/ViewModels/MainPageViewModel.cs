using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using WorkTracker.Classes;
using WorkTracker.Contracts;
using WorkTracker.Database.DTO;
using WorkTracker.Database.DTOs;
using WorkTracker.Events;
using WorkTracker.Services;
using WorkTracker.WebAccess.Implementations;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace WorkTracker.ViewModels
{
    public class MainPageViewModel : ViewModelBase, IDisposable
    {
        public delegate void AttendanceDateChanged();

        private readonly IAssignmentDataAccessService _assignmentDataAccessService;
        private readonly IEventAggregator _eventAggrigator;
        private readonly IJobDataAccessService _jobDataAccessService;
        private readonly ICachedDataService _cachedDataService;
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
        private DateTime? _userSelectedDate;
        private bool _isCommandActive;
        private ObservableCollection<JobDTO> _allJobs;
        private bool _isNoJob;
        private JobDTO _Job;
        private ICommand _addJobCommand;
        private OwnerDTO _owner;

        public MainPageViewModel(INavigationService navigationService, IPopupService popupService,
            IEventAggregator ea, INotificationService ns, IAssignmentDataAccessService assignmentDataAccessService,
            IWorkerDataAccessService workerDataAccessService, IJobDataAccessService jobDataAccessService,
            ICachedDataService cachedDataService)
            : base(navigationService)
        {
            Title = "WorkTrack";
            _eventAggrigator = ea;
            _notify = ns;
            _workerDataAccessService = workerDataAccessService;
            _assignmentDataAccessService = assignmentDataAccessService;
            _jobDataAccessService = jobDataAccessService;
            _cachedDataService = cachedDataService;
            _owner = _cachedDataService.GetCachedOwner();
            _popupService = popupService;
            AllAssignments = new ObservableCollection<UiBindableAssignment>();
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
            _addNewWorkerCommand ??=
                new DelegateCommand(ExecuteAddNewWorkerCommand, CanExecuteCommand).ObservesProperty(() =>
                    IsCommandActive);

        public DelegateCommand SubmitAttendanceCommand =>
            _submitAttendance ??=
                new DelegateCommand(ExecuteSubmitAttendanceCommand).ObservesProperty(() => IsCommandActive);

        public DelegateCommand<object> NavigateToSummaryCommand =>
            _navigateToSummaryCommand ??=
                new DelegateCommand<object>(ExecuteNavigateToSummaryCommand, CanExecuteCommand).ObservesProperty(() =>
                    IsCommandActive);

        public ICommand AddJobCommand =>
            _addJobCommand ??= new DelegateCommand(ExecuteAddJobCommand).ObservesProperty(() => IsCommandActive);

        public DelegateCommand<object> NavigateToJobAssignmentCommand =>
            _navigateToJobAssignmentCommand ??=
                new DelegateCommand<object>(ExecuteNavigateToJobAssignmentCommand, CanExecuteCommand).ObservesProperty(
                    () => IsCommandActive);

        private DelegateCommand<object> _navigateToJobStatisticsCommand;

        public DelegateCommand<object> NavigateToJobStatisticsCommand =>
            _navigateToJobStatisticsCommand ??
            (_navigateToJobStatisticsCommand =
                new DelegateCommand<object>(ExecuteNavigateToJobStatisticsCommand, CanExecuteCommand))
            .ObservesProperty(() => IsCommandActive);

        async void ExecuteNavigateToJobStatisticsCommand(object obj)
        {
            if (IsCommandActive)
                return;

            IsCommandActive = true;
            _popupService.ShowLoadingScreen();
            await NavigationService.NavigateAsync(Constants.JobStatistics, new NavigationParameters()
            {
                {Constants.Job, obj}
            });

            _popupService.HideLoadingScreen();
            IsCommandActive = false;
        }

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
            _popupService.ShowLoadingScreen();
            try
            {
                if (!SelectedDate.HasValue)
                    return;

                var date = SelectedDate.Value;
                var submittedAssignment =
                    await _assignmentDataAccessService.GetAllAssignment(_owner.Id, date, date,
                        null);

                if (!submittedAssignment.Any()) // If there is no attendance submitted for this date
                {
                    foreach (var assignment in AllAssignments)
                        assignment.IsAttendanceSubmitted =
                            false; //For all the pre-existing assignment on the UI, mark as not-submitted

                    IsAssignmentSubmittedAlready = false; //Mark assignment is not submitted to this date
                    return;
                }

                //If assignment is fetched for this date

                IsAssignmentSubmittedAlready = true; //Mark assignment is already submitted
                foreach (var assignmentToDeSelect in AllAssignments
                ) //For all the worker present in UI, clear the data. Data is added in next step
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


        private bool CanExecuteCommand()
        {
            return !IsCommandActive;
        }
        private bool CanExecuteCommand(object ob)
        {
            return !IsCommandActive;
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

        private async void ExecuteNavigateToJobAssignmentCommand(object obj)
        {
            if (IsCommandActive)
                return;

            IsCommandActive = true;
            _popupService.ShowLoadingScreen();

            var assignment = (UiBindableAssignment)obj;
            _selectedWorker = assignment.Assignment.Worker;
            await NavigationService.NavigateAsync(Constants.JobAssignmentPage,
                new NavigationParameters { { "Worker", _selectedWorker }, { "AllJobs", AllJobs?.ToList() }, { "JobsAssigned", assignment.AssignedJobs?.ToList() } });
            IsCommandActive = false;
            _popupService.HideLoadingScreen();
        }

        private void ExecuteAddNewWorkerCommand()
        {
            IsCommandActive = true;
            NavigationService.NavigateAsync(Constants.AddWorkerPage);
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            _ = parameters.TryGetValue("from", out string from);

            switch (from)
            {
                case Constants.JobAssignmentPage:
                    var jobs = parameters.GetValue<List<JobDTO>>("Jobs");
                    UpdateAssignmentWithNewlyAddedJobs(jobs);
                    break;
                case Constants.Login:
                    IsAssignmentSubmittedAlready = false;
                    AllWorkers = await GetAllWorkers();
                    AllAssignments = await PopulateAssignment(AllWorkers);
                    AllJobs = await GetAllJobs();
                    SubscribeEvents();
                    Job = new JobDTO();
                    SelectedDate = _cachedDataService.GetLastSubmittedDate();
                    IsNoWorker = AllWorkers.Count == 0;
                    break;
            }

            IsCommandActive = false;
        }

        private async Task<ObservableCollection<JobDTO>> GetAllJobs()
        {
            try
            {
                var result = await _jobDataAccessService.GetAllJob(_owner.Id);
                IsNoJob = result.Count == 0;
                return new ObservableCollection<JobDTO>(result);
            }
            catch (Exception e)
            {

                _notify.Notify(e.Message, NotificationTypeEnum.Error);
            }

            return new ObservableCollection<JobDTO>();
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
            _eventAggrigator.GetEvent<JobAddedEvent>().Subscribe(EventHandler);
            DateSelectionModified += FetchSubmittedAttendanceForSelectedDate;
        }

        public bool IsCommandActive
        {
            get => _isCommandActive;
            private set => SetProperty(ref _isCommandActive, value);
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
                    //var ownerId = Preferences.Get(Constants.UserId, 0);
                    //var workerList = await _workerDataAccessService.GetAllWorker(ownerId);
                    //AllWorkers = new ObservableCollection<WorkerDTO>(workerList);

                    AllAssignments.Add(new UiBindableAssignment()
                    {
                        IsAttendanceSubmitted = false,
                        AssignedJobs = new ObservableCollection<JobDTO>(),
                        IsSelected = false,
                        Assignment = new AssignmentDTO()
                        {
                            AssignedDate = DateTime.Now,
                            Worker = obj.Worker,
                            WorkerId = obj.Worker.Id
                        },

                    });
                    AllWorkers.Add(obj.Worker);
                    IsNoWorker = false;
                    break;
            }

            //AllAssignments = PopulateAssignment(AllWorkers);
        }

        private async Task<ObservableCollection<WorkerDTO>> GetAllWorkers()
        {
            try
            {
                var workerList = await _workerDataAccessService.GetAllWorker(_owner.Id);
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
        private async Task<ObservableCollection<UiBindableAssignment>> PopulateAssignment(ObservableCollection<WorkerDTO> workers)
        {
            var assignments = new ObservableCollection<UiBindableAssignment>();
            foreach (var worker in workers)
                assignments.Add(new UiBindableAssignment
                {
                    Assignment = new AssignmentDTO
                    {
                        AssignedDate = DateTime.Now,
                        Worker = worker,
                        WorkerId = worker.Id
                    },
                    IsSelected = true
                });
            return assignments;
        }

        private void ExecuteNavigateToSummaryCommand(object param)
        {
            if (param == null)
                return;

            IsCommandActive = true;
            var navPara = new NavigationParameters();
            navPara.Add("Worker", param);
            NavigationService.NavigateAsync(Constants.SummaryPage, navPara);
        }

        /// <summary>
        /// Add a new attendance/ update existing attendance
        /// </summary>
        private async void ExecuteSubmitAttendanceCommand()
        {
            if (IsCommandActive)
                return;

            IsCommandActive = true;
            try
            {
                _popupService.ShowLoadingScreen();
                await ValidateAttendanceForSubmission(); //Validation before adding assignment

                //Give a warning if attendance is submitted already for the chosen date
                if (IsAssignmentSubmittedAlready && !await _popupService.ShowPopup(Resource.Warning,
                    Resource.AttendanceSubmissionWarning, Resource.Yes,
                    Resource.No))
                {
                }
                else
                {

                    //Delete existing attendance if any.
                    await _assignmentDataAccessService.DeleteAssignments(_owner.Id, SelectedDate.Value);


                    var assignmentsToInsert = AllAssignments.Where(x => x.IsSelected).Select(x => new AssignmentDTO()
                    {
                        Jobs = x.AssignedJobs,
                        Wage = x.Wage ?? 0,
                        WorkerId = x.Assignment.Worker.Id,
                        OwnerId = _owner.Id,
                        AssignedDate = SelectedDate.Value
                    });
                    await _assignmentDataAccessService.InsertAssignment(assignmentsToInsert.ToList());

                    foreach (var uiBindableAssignment in AllAssignments.Where(x => x.IsSelected)) //Submit the attendance for selected workers only
                    {
                        uiBindableAssignment.IsAttendanceSubmitted = true;
                    }

                    IsAssignmentSubmittedAlready = true;
                    AllAssignments.Where(x => !x.IsSelected)?.ForEach(x => x.IsAttendanceSubmitted = false);
                    _notify.Notify("Success", NotificationTypeEnum.Success);
                    _cachedDataService.UpdateLastSubmittedDate(SelectedDate.Value);
                }
            }
            catch (WtException e)
            {
                _notify.Notify(e.Message, NotificationTypeEnum.Error);
            }
            catch (Exception e)
            {
                _notify.Notify(Resource.Failure, NotificationTypeEnum.Error);
            }
            finally
            {
                IsCommandActive = false;
                _popupService.HideLoadingScreen();
            }
        }

        private async Task<bool> ValidateAttendanceForSubmission()
        {
            var assignmentsToSubmit = AllAssignments.Where(x => x.IsSelected);

            if (!assignmentsToSubmit.ToList().Any()) throw new Exception("Unable to submit"); //If no assignment is selected for insertion

            var noJobAssignments = assignmentsToSubmit.Where(x => x.AssignedJobs == null || x.AssignedJobs.Count == 0);

            var assignments = noJobAssignments as UiBindableAssignment[] ?? noJobAssignments.ToArray();

            if (assignments.Any()) //Warn user for not adding job for assigning 
            {
                var workersName = assignments.Select(x => x.Assignment.Worker.Name);
                var enumerable = workersName as string[] ?? workersName.ToArray();
                var excep = new WtException();
                excep.Message = enumerable.Length <= 10
                    ? "Please assign at least one job to " + string.Join(",", enumerable)
                    : "Please assign at least one job to workers";

                throw excep;
            }

            return true;
        }

        private void EventHandler(JobDTO obj)
        {
            AllJobs.Insert(0, obj);
            RaisePropertyChanged(nameof(AllJobs));
        }

        /// <summary>
        /// Adds a new job using wep api cal
        /// </summary>
        private async void ExecuteAddJobCommand()
        {
            IsCommandActive = true;
            if (string.IsNullOrWhiteSpace(Job.Name))
            {
                _notify.Notify(Resource.JobNameError, NotificationTypeEnum.Error);
                IsCommandActive = false;
                return;
            }

            try
            {
                _popupService.ShowLoadingScreen();

                var addedJob = await _jobDataAccessService.InsertJob(_owner.Id, Job.Name);
                IsNoJob = false;
                _eventAggrigator.GetEvent<JobAddedEvent>().Publish(addedJob);
                _notify.Notify(Resource.JobAddedSuccess, NotificationTypeEnum.Success);
                Job = new JobDTO();
            }
            catch (WtException wt) when (wt.ErrorCode == Constants.DUPLICATE_JOBNAME)
            {
                _notify.Notify(Resource.DuplicateJobName, NotificationTypeEnum.Error);
            }
            catch (Exception e)
            {
                _notify.Notify(Resource.Failure,
                    NotificationTypeEnum.Error);
            }
            finally
            {
                _popupService.HideLoadingScreen();
                IsCommandActive = false;
            }
        }
    }
}