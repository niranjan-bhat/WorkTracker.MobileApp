using System;
using System.Collections.Generic;
using System.Text;
using WorkTracker.Classes;
using WorkTracker.Database.DTOs;

namespace WorkTracker.Events
{
    /// <summary>
    /// Event argument to be passed on WorkerModifiedEvent 
    /// </summary>
    public class WorkerModifiedEventArguments
    {
        public WorkerDTO Worker { get; set; }
        public CrudEnum ModificationType { get; set; }
    }
}
