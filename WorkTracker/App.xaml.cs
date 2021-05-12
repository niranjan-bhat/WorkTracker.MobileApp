using Prism;
using Prism.Ioc;
using Prism.Mvvm;
using WorkTracker.ViewModels;
using WorkTracker.Views;
using Xamarin.Essentials.Interfaces;
using Xamarin.Essentials.Implementation;
using Xamarin.Forms;
using Prism.Events;
using WorkTracker.Contracts;
using WorkTracker.Services;
using WorkTracker.WebAccess.Implementations;
using WorkTracker.WebAccess.Interfaces;
using WorkTracker.WebAccessLayer.Interfaces;

namespace WorkTracker
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
            Sharpnado.Shades.Initializer.Initialize(loggerEnable: false);
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            ViewModelLocationProvider.Register<JobView>(() => Container.Resolve<JobViewModel>());
            ViewModelLocationProvider.Register<PaymentView>(() => Container.Resolve<PaymentViewModel>());

            await NavigationService.NavigateAsync($"{Constants.Login}");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            try
            {
                containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

                containerRegistry.RegisterForNavigation<NavigationPage>(Constants.Navigation);
                containerRegistry.RegisterForNavigation<SummaryPage, SummaryPageViewModel>(Constants.SummaryPage);
                containerRegistry.RegisterForNavigation<JobAssignmentPage, JobAssignmentViewModel>(Constants.JobAssignmentPage);
                containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>(Constants.MainPage);
                containerRegistry.RegisterForNavigation<AddWorkerPage, AddWorkerPageViewModel>(Constants.AddWorkerPage);
                containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>(Constants.Login);
                containerRegistry.RegisterForNavigation<SignUpPage, SignUpViewModel>(Constants.SignUpPage);

                containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();
                containerRegistry.RegisterSingleton<JobViewModel>();
                containerRegistry.Register<INotificationService, NotificationService>();
                containerRegistry.Register<IOwnerDataAccessService, OwnerDataAccessService>();
                containerRegistry.Register<IWebDataAccess, WebDataAccess>();
                containerRegistry.RegisterSingleton(typeof(IConfiguration), typeof(WebConfig));
                containerRegistry.Register<IAuthorization, Authorization>();
                containerRegistry.Register<IEncryptionHelper, EncryptionHelper>();
                containerRegistry.Register<IMiscellaneousService, MiscellaneousService>();
                containerRegistry.Register<IPopupService, PopupService>();
                containerRegistry.Register<IWorkerDataAccessService, WorkerDataAccessService>();
                containerRegistry.Register<IJobDataAccessService, JobDataAccessService>();
                containerRegistry.Register<IAssignmentDataAccessService, AssignmentDataAccessService>();
            }
            catch (System.Exception e)
            {

            }
        }
    }
}
