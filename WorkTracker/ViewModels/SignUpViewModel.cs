using System;
using Prism.Commands;
using Prism.Navigation;
using WorkTracker.Classes;
using WorkTracker.Contracts;

namespace WorkTracker.ViewModels
{
    public class SignUpViewModel : ViewModelBase
    {
        private readonly IMiscellaneousService _miscellaneousService;
        private readonly INotificationService _notificationService;
        private readonly IOwnerDataAccessService _ownerService;
        private readonly IPopupService _popupservice;
        private string _confirmPassword;
        private bool _isOtpLayoutVisible;
        private int _otp;
        private int? _otpByUser;
        private string _password;
        private DelegateCommand _registerCommand;
        private string _userEmail;
        private string _userName;
        private DelegateCommand _verifyOtpCommand;
        private bool _isCommandActive;

        public SignUpViewModel(INavigationService navigationService, IOwnerDataAccessService ownerService,
            INotificationService ns, IPopupService popup, IMiscellaneousService miscellaneousService) : base(
            navigationService)
        {
            _ownerService = ownerService;
            _notificationService = ns;
            _popupservice = popup;
            _miscellaneousService = miscellaneousService;
            IsOtpLayoutVisible = false;
        }

        public bool IsOtpLayoutVisible
        {
            get => _isOtpLayoutVisible;
            set => SetProperty(ref _isOtpLayoutVisible, value);
        }

        public bool IsCommandActive
        {
            get => _isCommandActive;
            private set => SetProperty(ref _isCommandActive, value);
        }

        public int? OtpByUser
        {
            get => _otpByUser;
            set => SetProperty(ref _otpByUser, value);
        }

        public DelegateCommand VerifyOtpCommand =>
            _verifyOtpCommand ??= new DelegateCommand(ExecuteVerifyOtpCommand, CanExecuteCommand).ObservesProperty(() => IsCommandActive);

        public DelegateCommand RegisterCommand =>
            _registerCommand ??= new DelegateCommand(ExecuteRegisterCommand, CanExecuteCommand).ObservesProperty(() => IsCommandActive);

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
        private bool CanExecuteCommand()
        {
            return !IsCommandActive;
        }

        private async void ExecuteVerifyOtpCommand()
        {
            if (_otp == _otpByUser)
            {
                IsCommandActive = true;
                _popupservice.ShowLoadingScreen();
                try
                {
                    await _ownerService.VerifyEmail(UserEmail);
                    _notificationService.Notify(Resource.Login, NotificationTypeEnum.Success);

                    await NavigationService.GoBackAsync();
                }
                catch (Exception e)
                {
                    _notificationService.Notify(Resource.Failure, NotificationTypeEnum.Error);
                }
                finally
                {
                    _popupservice.HideLoadingScreen();
                }

                return;
            }

            _notificationService.Notify(Resource.OtpMismatch, NotificationTypeEnum.Error);
            IsCommandActive = false;
        }

        private async void ExecuteRegisterCommand()
        {
            if (string.IsNullOrEmpty(UserName))
            {
                _notificationService.Notify(string.Format(Resource.InvalidValue, "Username"),
                    NotificationTypeEnum.Error);
                return;
            }

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

            if (string.IsNullOrEmpty(ConfirmPassword))
            {
                _notificationService.Notify(string.Format(Resource.InvalidValue, "Confirm password"),
                    NotificationTypeEnum.Error);
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
                IsCommandActive = true;
                await _ownerService.Register(UserName, UserEmail, Password);

                _otp = await _miscellaneousService.GenerateOTPForUser(UserEmail);

                IsOtpLayoutVisible = true;

                _notificationService.Notify(string.Format(Resource.PleaseEnterOtp, UserEmail),
                    NotificationTypeEnum.Success);
            }
            catch (Exception e)
            {
                _notificationService.Notify(Resource.Failure, NotificationTypeEnum.Error);
            }
            finally
            {
                IsCommandActive = false;
                _popupservice.HideLoadingScreen();
            }
        }
    }
}