using System;
using System.Threading;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Navigation;
using WorkTracker.Classes;
using WorkTracker.Contracts;
using WorkTracker.WebAccess.Implementations;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WorkTracker.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private DelegateCommand _loginCommand;
        private DelegateCommand _registerCommand;
        private readonly INotificationService _notificationService;
        private readonly IOwnerDataAccessService _ownerService;

        private string _password ;
        private string _userEmail ;

        private IPopupService _popupservice;
        private bool _isLoginAction;
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

        public LoginPageViewModel(INavigationService navigationService, IOwnerDataAccessService ownerService,
            INotificationService ns, IPopupService popup) : base(navigationService)
        {
            _ownerService = ownerService;
            _notificationService = ns;
            _popupservice = popup;
        }

        public DelegateCommand LoginCommand =>
            _loginCommand ??= new DelegateCommand(ExecuteLoginCommand);
        public DelegateCommand RegisterCommand =>
            _registerCommand ??= new DelegateCommand(ExecuteRegisterCommand);

        private async void ExecuteRegisterCommand()
        {
            await NavigationService.NavigateAsync($"{Constants.Navigation}/{Constants.SignUpPage}");
        }


        private async void ExecuteLoginCommand()
        {
            if (string.IsNullOrEmpty(UserEmail))
            {
                _notificationService.Notify(string.Format(Resource.InvalidValue, "Email"), NotificationTypeEnum.Error);
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                _notificationService.Notify(string.Format(Resource.InvalidValue, "Password"), NotificationTypeEnum.Error);
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

                Preferences.Set(Constants.UserEmail, user.Email);
                Preferences.Set(Constants.UserId, user.Id);

                await NavigationService.NavigateAsync($"{Constants.Navigation}/{Constants.MainPage}", new NavigationParameters()
                {
                    {"from",Constants.Login }
                });
            }
            catch (Exception e)
            {
                _notificationService.Notify(e.Message, NotificationTypeEnum.Error);
            }
            finally
            {
                _popupservice.HideLoadingScreen();
            }
        }
    }
}