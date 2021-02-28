using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkTracker.Events
{
    public class WorkerModifiedEvent: PubSubEvent<WorkerModifiedEventArguments> 
    {
    }
}
