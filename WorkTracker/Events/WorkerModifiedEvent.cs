using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkTracker.Events
{
    /// <summary>
    /// Event to be raised when user adds,updates a worker
    /// </summary>
    public class WorkerModifiedEvent: PubSubEvent<WorkerModifiedEventArguments> 
    {
    }
}
