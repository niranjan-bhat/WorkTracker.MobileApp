using System;
using Prism.Commands;
using Prism.Navigation;
using WorkTracker.Classes;
using WorkTracker.Contracts;
using WorkTracker.Services;
using WorkTracker.WebAccess.Implementations;
using Xamarin.Essentials;

namespace WorkTracker.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly INotificationService _notificationService;
        private readonly IOwnerDataAccessService _ownerService;

        private readonly IPopupService _popupservice;
        private readonly ICachedDataService _cacheService;
        private bool _isCommandActive;
        private DelegateCommand _loginCommand;

        private string _password;
        private DelegateCommand _registerCommand;
        private string _userEmail;

        public LoginPageViewModel(INavigationService navigationService, IOwnerDataAccessService ownerService,
            INotificationService ns, IPopupService popup, ICachedDataService cacheService) : base(navigationService)
        {
            _ownerService = ownerService;
            _notificationService = ns;
            _popupservice = popup;
            _cacheService = cacheService;
        }

        public bool IsCommandActive
        {
            get => _isCommandActive;
            private set => SetProperty(ref _isCommandActive, value);
        }

        public string UserEmail
        {
            get => _userEmail;
            set => SetProperty(ref _userEmail, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public DelegateCommand LoginCommand =>
            _loginCommand ??=
                new DelegateCommand(ExecuteLoginCommand, CanExecuteCommand).ObservesProperty(() => IsCommandActive);

        public DelegateCommand RegisterCommand =>
            _registerCommand ??=
                new DelegateCommand(ExecuteRegisterCommand, CanExecuteCommand).ObservesProperty(() => IsCommandActive);

        private bool CanExecuteCommand()
        {
            return !IsCommandActive;
        }

        private async void ExecuteRegisterCommand()
        {
            IsCommandActive = true;
            await NavigationService.NavigateAsync($"{Constants.Navigation}/{Constants.SignUpPage}");
        }


        private async void ExecuteLoginCommand()
        {
            IsCommandActive = true;
            if (string.IsNullOrEmpty(UserEmail))
            {
                _notificationService.Notify(string.Format(Resource.InvalidValue, "Email"), NotificationTypeEnum.Error);
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                _notificationService.Notify(string.Format(Resource.InvalidValue, "Password"),
                    NotificationTypeEnum.Error);
                return;
            }

            _popupservice.ShowLoadingScreen();
            try
            {
                if (!await _ownerService.Login(UserEmail, Password))
                {
                    _notificationService.Notify(Resource.LoginFailed, NotificationTypeEnum.Error);
                    return;
                }

                var user = await _ownerService.GetOwnerByEmail(UserEmail);

                _cacheService.CacheOwner(user);

                await NavigationService.NavigateAsync($"{Constants.Navigation}/{Constants.MainPage}",
                    new NavigationParameters
                    {
                        {"from", Constants.Login}
                    });
            }
            catch (WtException wt) when (wt.ErrorCode == Constants.USERNAMEPASSWORD_WRONG)
            {
                _notificationService.Notify(Resource.UsernamePasswordMismatch, NotificationTypeEnum.Error);
                IsCommandActive = false;
            }
            catch (Exception e)
            {
                _notificationService.Notify(Resource.LoginFailed, NotificationTypeEnum.Error);
                IsCommandActive = false;
            }
            finally
            {
                _popupservice.HideLoadingScreen();
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            IsCommandActive = false;
            base.OnNavigatedTo(parameters);
        }
    }
}