using System;
using System.Collections.Generic;
using System.Text;
using Prism.Events;
using WorkTracker.Database.DTOs;

namespace WorkTracker.Events
{
    /// <summary>
    /// Event to be raised when user adds a new job
    /// </summary>
    public class JobAddedEvent : PubSubEvent<JobDTO>
    {
    }
}
