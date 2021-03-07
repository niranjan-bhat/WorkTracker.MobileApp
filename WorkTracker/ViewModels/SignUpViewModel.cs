using Prism.Commands;
using Prism.Navigation;
using System;
using WorkTracker.Classes;
using WorkTracker.Contracts;

namespace WorkTracker.ViewModels
{
    public class SignUpViewModel : ViewModelBase
    {
        private string _confirmPassword;
        private string _password;
        private string _userEmail;
        private string _userName;
        private INotificationService _notificationService;
        private IOwnerDataAccessService _ownerService;
        private IPopupService _popupservice;
        private DelegateCommand _registerCommand;

        public DelegateCommand RegisterCommand =>
            _registerCommand ??= new DelegateCommand(ExecuteRegisterCommand);

        private async void ExecuteRegisterCommand()
        {
            if (string.IsNullOrEmpty(UserName))
            {
                _notificationService.Notify(string.Format(Resource.InvalidValue, "Username"), NotificationTypeEnum.Error);
                return;
            }
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
            if (string.IsNullOrEmpty(ConfirmPassword))
            {
                _notificationService.Notify(string.Format(Resource.InvalidValue, "Confirm password"), NotificationTypeEnum.Error);
                return;
            }
            if (!string.Equals(Password, ConfirmPassword))
            {
                _notificationService.Notify(Resource.PasswordMismatch, NotificationTypeEnum.Error);
                return;
            }

            _popupservice.ShowLoadingScreen();
            try
            {
                await _ownerService.Register(UserName, UserEmail, Password);

               await NavigationService.GoBackAsync();

               _notificationService.Notify(Resource.RegistrationSuccess, NotificationTypeEnum.Success);
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

        public SignUpViewModel(INavigationService navigationService, IOwnerDataAccessService ownerService,
            INotificationService ns, IPopupService popup) : base(navigationService)
        {
            _ownerService = ownerService;
            _notificationService = ns;
            _popupservice = popup;
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

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }
    }
}