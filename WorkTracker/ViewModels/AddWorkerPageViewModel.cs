using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using WorkTracker.Classes;
using WorkTracker.Contracts;
using WorkTracker.Database.DTOs;
using WorkTracker.Events;
using WorkTracker.WebAccess.Implementations;
using Xamarin.Essentials;

namespace WorkTracker.ViewModels
{
    public class AddWorkerPageViewModel : ViewModelBase
    {
        private readonly IEventAggregator _ea;
        private readonly IWorkerDataAccessService _workerDAService;
        private readonly IPopupService _popupservice;
        private WorkerDTO _newWorker;
        private readonly INotificationService _notificationService;
        private ICommand _submitCommand;
        private bool _isCommandActive;

        public AddWorkerPageViewModel(INavigationService navigationService, IEventAggregator ea,
            IWorkerDataAccessService dataAccessService, IPopupService popupservice, INotificationService notificationService) : base(
            navigationService)
        {
            Title = "Add a new worker";
            _ea = ea;
            NewWorker = new WorkerDTO();
            _workerDAService = dataAccessService;
            _popupservice = popupservice;
            _notificationService = notificationService;
        }

        public bool IsCommandActive
        {
            get => _isCommandActive;
            private set => SetProperty(ref _isCommandActive, value);
        }

        public WorkerDTO NewWorker
        {
            get => _newWorker;
            set => SetProperty(ref _newWorker, value);
        }

        public ICommand SubmitCommand =>
            _submitCommand ??= new DelegateCommand(ExecuteSubmitCommand);

        /// <summary>
        /// Function to add a new worker
        /// </summary>
        private async void ExecuteSubmitCommand()
        {
            try
            {
                _popupservice.ShowLoadingScreen();
                var result = await _workerDAService.AddWorker(Preferences.Get(Constants.UserId, 0),
                    FormatString(NewWorker.Name),
                    NewWorker.Mobile);

                _ea.GetEvent<WorkerModifiedEvent>().Publish(new WorkerModifiedEventArguments
                {
                    ModificationType = CrudEnum.Added,
                    Worker = result
                });
                _notificationService.Notify(Resource.UserAddedSuccess, NotificationTypeEnum.Success);
                NewWorker = new WorkerDTO();
            }
            catch (WtException wt) when (wt.ErrorCode == Constants.DUPLICATE_MOBILE_NUMBER)
            {
                _notificationService.Notify(Resource.DuplicateMobileNumber, NotificationTypeEnum.Error);

            }
            catch (WtException wt) when (wt.ErrorCode == Constants.DUPLICATE_WORKERNAME)
            {
                _notificationService.Notify(Resource.DuplicateWorkerName, NotificationTypeEnum.Error);

            }
            catch (Exception e)
            {
                _notificationService.Notify(Resource.Failure, NotificationTypeEnum.Error);
            }
            finally
            {
                _popupservice.HideLoadingScreen();
            }
        }

        /// <summary>
        /// Returns the string with uppercase starting letter 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string FormatString(string str)
        {
            return str == null ? string.Empty : str.Substring(0, 1).ToUpper() + str.Substring(1)?.ToLower();
        }
    }
}