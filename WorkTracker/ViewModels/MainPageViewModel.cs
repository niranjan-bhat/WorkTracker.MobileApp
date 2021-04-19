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
        private bool _isNoWorker;
        private DelegateCommand<object> _navigateToJobAssignmentCommand;
        private DelegateCommand<object> _navigateToSummaryCommand;
        private WorkerDTO _selectedWorker;
        private DelegateCommand _submitAttendance;
        private DateTime? _userSelectedDate = DateTime.Today;
        private bool _isAssignmentSubmittedAlreadies;
        public bool IsAssignmentSubmittedAlready
        {
            get { return _isAssignmentSubmittedAlreadies; }
            set { SetProperty(ref _isAssignmentSubmittedAlreadies, value); }
        }

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

        public bool IsNoWorker
        {
            get => _isNoWorker;
            set => SetProperty(ref _isNoWorker, value);
        }

        public DelegateCommand<BackButtonPressedEventArgs> BackButtonPressCommand =>
            _backButtonPressCommand ??= new DelegateCommand<BackButtonPressedEventArgs>(ExecuteBackButtonPressCommand);

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
            _addNewWorkerCommand ?? (_addNewWorkerCommand = new DelegateCommand(ExecuteAddNewWorkerCommand));

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

        public event AttendanceDateChanged DateSelectionModified;

        private async void FetchSubmittedAttendanceForSelectedDate()
        {
            var ownerId = Preferences.Get(Constants.UserId, 0);
            _popupService.ShowLoadingScreen();
            try
            {
                var submittedAssignment =
                    await _assignmentDataAccessService.GetAllAssignment(ownerId, SelectedDate.Value, SelectedDate.Value,
                        null);

                if (!submittedAssignment.Any())
                {
                    foreach (var assignment in AllAssignments)
                    {
                        assignment.IsAttendanceSubmitted = false;
                    }
                    IsAssignmentSubmittedAlready = false;
                    return;
                }

                IsAssignmentSubmittedAlready = true;
                foreach (var assignmentToDeSelect in AllAssignments)
                {
                    assignmentToDeSelect.IsSelected = false;
                    assignmentToDeSelect.IsAttendanceSubmitted = false;
                    assignmentToDeSelect.AssignedJobs?.Clear();
                    assignmentToDeSelect.Wage = null;
                }

                foreach (var assignment in submittedAssignment)
                {
                    var assignmentForWorker =
                        AllAssignments.FirstOrDefault(x => x.Assignment.Worker.Id == assignment.WorkerId);
                    assignmentForWorker.Wage = assignment.Wage;
                    assignmentForWorker.IsAttendanceSubmitted = true;
                    assignmentForWorker.AssignedJobs = new ObservableCollection<JobDTO>(assignment.Jobs);
                    assignmentForWorker.IsSelected = true;
                    assignmentForWorker.Assignment.Id = assignment.Id;
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                _popupService.HideLoadingScreen();
            }
        }

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
                    HandleAssignedJobs(jobs);
                    break;
                case Constants.Login:
                    IsAssignmentSubmittedAlready = false;
                    AllWorkers = await GetAllWorkers();
                    AllAssignments = PopulateAssignment(AllWorkers);
                    SubscribeEvents();
                    IsNoWorker = AllWorkers.Count == 0;
                    SelectedDate = Preferences.Get(Constants.LatestDateOfAttendanceSubmission, DateTime.Today);
                    break;
            }
        }

        private void HandleAssignedJobs(List<JobDTO> jobs)
        {
            var assignment = AllAssignments.FirstOrDefault(x => x.Assignment.Worker.Id == _selectedWorker.Id);
            assignment.AssignedJobs = new ObservableCollection<JobDTO>(jobs);
        }

        private void SubscribeEvents()
        {
            _eventAggrigator.GetEvent<WorkerModifiedEvent>().Subscribe(WorkerModifiedEventHandler);
            DateSelectionModified += FetchSubmittedAttendanceForSelectedDate;
        }

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
            }

            return new ObservableCollection<WorkerDTO>();
        }

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

        private async void ExecuteSubmitAttendanceCommand()
        {
            IsBusy = true;
            try
            {
                await ValidateAttendanceForSubmission();

                if (IsAssignmentSubmittedAlready && !await _popupService.ShowPopup(Resource.Warning,
                    Resource.AttendanceSubmissionWarning, Resource.Yes,
                    Resource.No))
                {
                    return;
                }
                else
                {
                    var ownerId = Preferences.Get(Constants.UserId, 0);
                    await _assignmentDataAccessService.DeleteAssignments(ownerId, SelectedDate.Value);

                    foreach (var uiBindableAssignment in AllAssignments.Where(x => x.IsSelected))
                    {
                       

                        if (!uiBindableAssignment.IsAttendanceSubmitted)
                        {
                            await _assignmentDataAccessService.InsertAssignment(ownerId,
                                uiBindableAssignment.Wage.HasValue
                                    ? uiBindableAssignment.Wage.Value
                                    : 0,
                                uiBindableAssignment.Assignment.Worker.Id, SelectedDate.Value,
                                uiBindableAssignment.AssignedJobs?.ToList());

                            uiBindableAssignment.IsAttendanceSubmitted = true;
                        }
                    }

                    IsAssignmentSubmittedAlready = true;
                    _notify.Notify("Success", NotificationTypeEnum.Success);
                    Preferences.Set(Constants.LatestDateOfAttendanceSubmission, SelectedDate.Value);
                }
            }
            catch (Exception e)
            {
                _notify.Notify(e.Message, NotificationTypeEnum.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task<bool> ValidateAttendanceForSubmission()
        {
            var assignmentsToSubmit = AllAssignments.Where(x => x.IsSelected);

            if (!assignmentsToSubmit.ToList().Any()) throw new Exception("Unable to submit");

            var noJobAssignments = assignmentsToSubmit.Where(x => x.AssignedJobs == null);

            var assignments = noJobAssignments as UiBindableAssignment[] ?? noJobAssignments.ToArray();

            if (assignments.Any())
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