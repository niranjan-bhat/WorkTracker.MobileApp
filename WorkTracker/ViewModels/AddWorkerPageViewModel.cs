using System;
using System.Collections.Generic;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using WorkTracker.Classes;
using WorkTracker.Contracts;
using WorkTracker.Database.DTOs;
using WorkTracker.Events;
using Xamarin.Essentials;

namespace WorkTracker.ViewModels
{
    public class AddWorkerPageViewModel : ViewModelBase
    {
        private readonly IEventAggregator _ea;
        private WorkerDTO _newWorker;
        private ICommand _submitCommand;
        private readonly IWorkerDataAccessService _workerDAService;

        public AddWorkerPageViewModel(INavigationService navigationService, IEventAggregator ea,
            IWorkerDataAccessService dataAccessService) : base(navigationService)
        {
            Title = "Add a new worker";
            _ea = ea;
            NewWorker = new WorkerDTO();
            _workerDAService = dataAccessService;
        }

        public WorkerDTO NewWorker
        {
            get => _newWorker;
            set => SetProperty(ref _newWorker, value);
        }

        public ICommand SubmitCommand =>
            _submitCommand ??= new DelegateCommand(ExecuteSubmitCommand);

        private async void ExecuteSubmitCommand()
        {
            try
            {
                var result = await _workerDAService.AddWorker(Preferences.Get(Constants.UserId, 0),
                    FormatString(NewWorker.Name),
                    NewWorker.Mobile);

                _ea.GetEvent<WorkerModifiedEvent>().Publish(new WorkerModifiedEventArguments
                {
                    ModificationType = CrudEnum.Added,
                    Worker = result
                });

                NewWorker = new WorkerDTO();
            }
            catch (Exception e)
            {
            }
        }

        private string FormatString(string str)
        {
            return str == null ? string.Empty : str.Substring(0, 1).ToUpper() + str.Substring(1)?.ToLower();
        }
    }
}