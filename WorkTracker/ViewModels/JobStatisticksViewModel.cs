using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Navigation;
using Telerik.XamarinForms.Chart;
using Telerik.XamarinForms.DataGrid.Commands;
using WorkTracker.Classes;
using WorkTracker.Contracts;
using WorkTracker.Database.DTOs;
using WorkTracker.Services;
using WorkTracker.WebAccess.Implementations;
using Xamarin.Essentials;

namespace WorkTracker.ViewModels
{
    public class JobStatisticksViewModel : ViewModelBase
    {
        private readonly IAssignmentDataAccessService _assignmentDataAccessService;
        private readonly INotificationService _notificationService;
        private readonly IPopupService _popupService;
        private readonly IWorkerDataAccessService _workerDataAccessService;
        private readonly ICachedDataService _cachedDataService;
        private DelegateCommand _calculateStatisticsCommand;
        private DateTime? _endDateTime;

        private bool _isTotalWageVisible;
        private ObservableCollection<JobStatisticsChartModel> _itemSource;
        private JobDTO _job;
        private ChartPalette _palette;

        private DateTime? _startDateTime;
        private int? _totalMoneySpentForThisJob;
        private Dictionary<string, Color> _workerColorDictionary = new Dictionary<string, Color>();

        public JobStatisticksViewModel(INavigationService navigationService, IPopupService popupService,
            INotificationService notificationService, IAssignmentDataAccessService assignmentDataAccessService,
            IWorkerDataAccessService workerDataAccessService, ICachedDataService cachedDataService) : base(navigationService)
        {
            _popupService = popupService;
            _notificationService = notificationService;
            _assignmentDataAccessService = assignmentDataAccessService;
            _workerDataAccessService = workerDataAccessService;
            _cachedDataService = cachedDataService;
        }

        public bool IsTotalWageVisible
        {
            get => _isTotalWageVisible;
            set => SetProperty(ref _isTotalWageVisible, value);
        }

        public int? TotalMoneySpentForThisJob
        {
            get => _totalMoneySpentForThisJob;
            set => SetProperty(ref _totalMoneySpentForThisJob, value);
        }

        public ChartPalette ChartPaletteCollection
        {
            get => _palette;
            set => SetProperty(ref _palette, value);
        }

        public ObservableCollection<JobStatisticsChartModel> ItemSource
        {
            get => _itemSource;
            set => SetProperty(ref _itemSource, value);
        }

        public DateTime? EndDate
        {
            get => _endDateTime;
            set => SetProperty(ref _endDateTime, value);
        }

        public DateTime? StartDate
        {
            get => _startDateTime;
            set => SetProperty(ref _startDateTime, value);
        }

        public JobDTO Job
        {
            get => _job;
            set => SetProperty(ref _job, value);
        }

        public DelegateCommand CalculateStatistics =>
            _calculateStatisticsCommand ??
            (_calculateStatisticsCommand = new DelegateCommand(ExecuteCalculateStatistics));

        private async void ExecuteCalculateStatistics()
        {
            _popupService.ShowLoadingScreen();

            try
            {
                ValidateInput();

                var ownerId = _cachedDataService.GetCachedOwner().Id;
                var allAssignment =
                    await _assignmentDataAccessService.GetAllAssignment(ownerId, StartDate.Value, EndDate.Value, null,
                        Job.Id);

                if (allAssignment.Count == 0)
                {
                    ItemSource?.Clear();
                    throw new Exception(Resource.NoJobStatistics);
                }

                var allWorkers = await _workerDataAccessService.GetAllWorker(ownerId);

                var listChartModels = allAssignment.GroupBy(x => x.WorkerId).Select(y => new JobStatisticsChartModel
                {
                    Days = y.Count(),
                    WorkerName = allWorkers.FirstOrDefault(x => x.Id == y.Key).Name
                }).ToList();

                listChartModels = listChartModels.OrderBy(x => x.Days).Reverse().ToList();

                foreach (var entry in listChartModels)
                    entry.TotalWage = allAssignment.Where(y =>
                            y.WorkerId == allWorkers.FirstOrDefault(z => z.Name == entry.WorkerName).Id)?
                        .Sum(x => x.Wage);

                TotalMoneySpentForThisJob = allAssignment.Sum(x => x.Wage);

                var colorList = CreateRandomPalettes(listChartModels.Count());

                ChartPaletteCollection = CreatePaletteFromColors(colorList);

                listChartModels = GenerateWorkerColorDictionary(colorList, listChartModels);

                ItemSource = new ObservableCollection<JobStatisticsChartModel>(listChartModels);

                IsTotalWageVisible = ItemSource.Count != 0;
            }
            catch (Exception e)
            {
                string msg = (e is WtException wt) ? wt.Message : e.Message;
                IsTotalWageVisible = false;
                _notificationService.Notify(msg, NotificationTypeEnum.Error);
            }
            finally
            {
                _popupService.HideLoadingScreen();
            }
        }

        private void ValidateInput()
        {
            if (!StartDate.HasValue)
                throw new Exception(Resource.StartDateError);

            if (!EndDate.HasValue)
                throw new Exception(Resource.EndDateError);

            if (StartDate > EndDate)
                throw new Exception(Resource.NegativeDateRange);
        }

        private ChartPalette CreatePaletteFromColors(List<Color> colorList)
        {
            var chartPalette = new ChartPalette();
            colorList.ForEach(x =>
                chartPalette.Entries.Add(new PaletteEntry(x, x)));

            return chartPalette;
        }

        private PaletteEntry GetPalette()
        {
            return new PaletteEntry(Xamarin.Forms.Color.Brown, Xamarin.Forms.Color.Tomato);
        }

        private List<JobStatisticsChartModel> GenerateWorkerColorDictionary(List<Color> colorList,
            List<JobStatisticsChartModel> chartBindableList)
        {
            for (var i = 0; i < chartBindableList.Count(); i++)
                chartBindableList[i].ColorCode = colorList[i];

            return chartBindableList;
        }

        private List<Color> CreateRandomPalettes(int count)
        {
            var colors = new List<Color>();
            var r = new Random();
            for (var i = 0; i < count; i++)
            {
                var randomColor = Color.FromArgb(r.Next(0, 256),
                    r.Next(0, 256), r.Next(0, 256));

                colors.Add(randomColor);
            }

            return colors;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.TryGetValue(Constants.Job, out JobDTO job)) Job = job;
            Title = "Job statistics -  " + Job.Name;
        }


        private async Task GetAllAssignment()
        {
        }
    }
}