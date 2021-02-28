using System;
using System.Collections.Generic;
using System.Text;
using WorkTracker.Classes;

namespace WorkTracker.Contracts
{
    public interface  INotificationService
    {
        void Notify(string message, NotificationTypeEnum type);
    }
}
