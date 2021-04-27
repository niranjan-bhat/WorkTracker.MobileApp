using System;
using System.Collections.Generic;
using System.Text;
using WorkTracker.Classes;

namespace WorkTracker.Contracts
{
    public interface  INotificationService
    {
        /// <summary>
        /// Notifies the user
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        void Notify(string message, NotificationTypeEnum type);
    }
}
