using System;
using System.Collections.Generic;
using System.Text;
using Prism.Events;
using WorkTracker.Database.DTOs;

namespace WorkTracker.Events
{
    public class JobAddedEvent : PubSubEvent<JobDTO>
    {
    }
}
