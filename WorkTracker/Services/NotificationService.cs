using System;
using System.Collections.Generic;
using System.Text;
using Acr.UserDialogs;
using WorkTracker.Classes;
using WorkTracker.Contracts;
using Xamarin.Forms;

namespace WorkTracker.Services
{
    public class NotificationService : INotificationService
    {
        public void Notify(string message, NotificationTypeEnum type)
        {
            var config = new ToastConfig(message)
            {
                MessageTextColor = Color.White,
                Duration = new TimeSpan(0,0,0,2),
                BackgroundColor = type == NotificationTypeEnum.Success
                    ? Color.FromHex("#66CE89")
                    : Color.FromHex("#EC7063"),
                Position = ToastPosition.Top
            };
            UserDialogs.Instance.Toast(config);
        }
    }
}
