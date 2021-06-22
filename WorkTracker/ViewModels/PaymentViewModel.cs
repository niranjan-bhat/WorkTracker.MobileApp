using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Telerik.XamarinForms.Input.Calendar;
using WorkTracker.Classes;
using WorkTracker.Contracts;
using WorkTracker.DataAccess.DTOs;
using WorkTracker.Database.DTOs;
using WorkTracker.Events;
using WorkTracker.Services;
using Xamarin.Essentials;

namespace WorkTracker.ViewModels
{
    public class PaymentViewModel : BindableBase
    {
        private readonly IEventAggregator _iaEventAggregator;
        private readonly INotificationService _notifyService;
        private readonly IPaymentService _paymentService;
        private readonly IPopupService _popupService;
        private readonly IWorkerDataAccessService _workerDataAccessService;
        private readonly ICachedDataService _cachedDataService;

        private ICommand _activatetabCommand;
        private DelegateCommand _addPaymentCommand;
        private int _calculatedSalaryString;
        private DelegateCommand _calculateSalaryCommand;
        private bool _isCommandActive;
        private bool _isLoanPaymentType;

        private bool _isRePaymentPaymentType;

        private bool _isSalaryTabActive;
        private int? _paymentAmount;
        private ObservableCollection<PaymentDTO> _paymentCollection;
        private DateTimeRange _selecteDateTimeRange;
        private WorkerDTO _selectedWorkerDto;

        private int? _totalLoan;

        private int? _totalDue;
        private ObservableCollection<WorkerDTO> _workerList;

        public PaymentViewModel(IPaymentService paymentService, INotificationService notifyService,
            IPopupService popupService, IWorkerDataAccessService workerDataAccessService, ICachedDataService cachedDataService,
            IEventAggregator iaEventAggregator)
        {
            _paymentService = paymentService;
            _notifyService = notifyService;
            _popupService = popupService;
            _workerDataAccessService = workerDataAccessService;
            _cachedDataService = cachedDataService;
            _iaEventAggregator = iaEventAggregator;
            IsSalaryTabActive = true;
            Task.Run(GetAllWorkers);
            PaymentCollection = new ObservableCollection<PaymentDTO>();
            SubscribeEvents();
        }

        public int? TotalLoan
        {
            get => _totalLoan;
            set => SetProperty(ref _totalLoan, value);
        }

        public int? TotalDue
        {
            get => _totalDue;
            set => SetProperty(ref _totalDue, value);
        }


        public bool IsCommandActive
        {
            get => _isCommandActive;
            private set => SetProperty(ref _isCommandActive, value);
        }

        public int? PaymentAmount
        {
            get => _paymentAmount;
            set => SetProperty(ref _paymentAmount, value);
        }

        public DelegateCommand AddPaymentCommand =>
            _addPaymentCommand ??= new DelegateCommand(ExecuteAddPaymentCommand, CanExecuteCommand)
                .ObservesProperty(() => IsCommandActive);

        public bool IsLoanPaymentType
        {
            get => _isLoanPaymentType;
            set
            {
                if (_isLoanPaymentType != value)
                    SetProperty(ref _isLoanPaymentType, value);
            }
        }

        public bool IsRePaymentPaymentType
        {
            get => _isRePaymentPaymentType;
            set
            {
                if (_isRePaymentPaymentType != value)
                    SetProperty(ref _isRePaymentPaymentType, value);
            }
        }


        public int CalculatedSalaryString
        {
            get => _calculatedSalaryString;
            set => SetProperty(ref _calculatedSalaryString, value);
        }

        public DelegateCommand CalculateSalaryCommand =>
            _calculateSalaryCommand ??= new DelegateCommand(ExecuteCalculateSalaryCommand, CanExecuteCommand)
                .ObservesProperty(() => IsCommandActive);

        public DateTimeRange SelectedDateRange
        {
            get => _selecteDateTimeRange;
            set => SetProperty(ref _selecteDateTimeRange, value);
        }

        public WorkerDTO SelectedWorker
        {
            get => _selectedWorkerDto;
            set
            {
                SetProperty(ref _selectedWorkerDto, value);
                Task.Run(GetPaymentDetails);
            }
        }

        public ObservableCollection<PaymentDTO> PaymentCollection
        {
            get => _paymentCollection;
            set => SetProperty(ref _paymentCollection, value);
        }

        public ObservableCollection<WorkerDTO> AllWorkers
        {
            get => _workerList;
            set => SetProperty(ref _workerList, value);
        }

        public bool IsSalaryTabActive
        {
            get => _isSalaryTabActive;
            set => SetProperty(ref _isSalaryTabActive, value);
        }

        public ICommand ActivatetabCommand =>
            _activatetabCommand ??= new DelegateCommand<object>(obj =>
            {
                var tab = (PaymentPageTabsEnum)obj;
                IsSalaryTabActive = tab switch
                {
                    PaymentPageTabsEnum.Payment => false,
                    PaymentPageTabsEnum.Salary => true,
                    _ => IsSalaryTabActive
                };
            });

        private bool CanExecuteCommand()
        {
            return !IsCommandActive;
        }

        private async void ExecuteAddPaymentCommand()
        {
            if (SelectedWorker == null)
            {
                _notifyService.Notify(Resource.WorkerSelectionWarning, NotificationTypeEnum.Error);
                return;
            }

            if (!(IsLoanPaymentType ^ IsRePaymentPaymentType))
            {
                var msg = IsLoanPaymentType && IsRePaymentPaymentType
                    ? Resource.PaymentSelectionOnlyOneWarning
                    : Resource.PaymentSelectionAtleastOneWarning;

                _notifyService.Notify(msg, NotificationTypeEnum.Error);
                return;
            }

            if (PaymentAmount <= 0)
            {
                _notifyService.Notify(Resource.PaymentAmountInvalidWarning, NotificationTypeEnum.Error);
                return;
            }

            try
            {
                IsCommandActive = true;
                _popupService.ShowLoadingScreen();
                var paymentObj = new PaymentDTO
                {
                    Amount = PaymentAmount.Value,
                    PaymentType = IsLoanPaymentType ? PaymentType.Loan : PaymentType.RePayment,
                    WorkerId = SelectedWorker.Id,
                    TransactionDate = DateTime.Today
                };

                await _paymentService.AddPayment(paymentObj);
                PaymentCollection.Add(paymentObj);
                CalculateTotalPaymentDone();
                _popupService.HideLoadingScreen();
                _notifyService.Notify(Resource.SuccessfulPayment, NotificationTypeEnum.Success);
            }
            catch (Exception e)
            {
                _notifyService.Notify(e.Message, NotificationTypeEnum.Error);
            }
            finally
            {
                IsCommandActive = false;
            }
        }

        private async void ExecuteCalculateSalaryCommand()
        {
            if (SelectedWorker == null)
            {
                _notifyService.Notify(Resource.WorkerSelectionWarning, NotificationTypeEnum.Error);
                return;
            }

            if (SelectedDateRange == null)
            {
                _notifyService.Notify(Resource.DateRangeSelectionWarning, NotificationTypeEnum.Error);
                return;
            }

            try
            {
                _popupService.ShowLoadingScreen();
                IsCommandActive = true;
                CalculatedSalaryString = await _workerDataAccessService.CalculateSalary(SelectedWorker.Id,
                    SelectedDateRange.From, SelectedDateRange.To);
            }
            catch (Exception e)
            {
                _notifyService.Notify(e.Message, NotificationTypeEnum.Error);
            }
            finally
            {
                IsCommandActive = false;
                _popupService.HideLoadingScreen();
            }
        }

        private async void GetPaymentDetails()
        {
            _popupService.ShowLoadingScreen();
            try
            {
                var result = await _paymentService.GetAllPayments(SelectedWorker.Id);

                PaymentCollection.Clear();
                PaymentCollection = new ObservableCollection<PaymentDTO>(result);
                CalculateTotalPaymentDone();
            }
            catch (Exception e)
            {
                _notifyService.Notify(e.Message, NotificationTypeEnum.Error);
            }
            finally
            {
                _popupService.HideLoadingScreen();
            }
        }

        private async void GetAllWorkers()
        {
            var ownerId = _cachedDataService.GetCachedOwner().Id;
            var list = await _workerDataAccessService.GetAllWorker(ownerId);

            AllWorkers = new ObservableCollection<WorkerDTO>(list);
        }

        private void SubscribeEvents()
        {
            _iaEventAggregator.GetEvent<WorkerModifiedEvent>().Subscribe(WorkerModifiedEventHandler);
        }

        /// <summary>
        ///     Handle worker data modified/added
        /// </summary>
        /// <param name="obj"></param>
        private async void WorkerModifiedEventHandler(WorkerModifiedEventArguments obj)
        {
            switch (obj.ModificationType)
            {
                case CrudEnum.Added:
                    AllWorkers.Add(obj.Worker);
                    break;
            }
        }

        private void CalculateTotalPaymentDone()
        {
            TotalLoan = PaymentCollection.Where(x => x.PaymentType == PaymentType.Loan)?.Sum(x => x.Amount);
            TotalDue = TotalLoan - PaymentCollection.Where(x => x.PaymentType == PaymentType.RePayment)?.Sum(x => x.Amount);
        }
    }
}