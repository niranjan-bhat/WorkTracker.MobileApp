using System;
using System.Collections.Generic;
using System.Text;
using WorkTracker.Classes;
using WorkTracker.Database.DTOs;

namespace WorkTracker.Events
{
    public class WorkerModifiedEventArguments
    {
        public WorkerDTO Worker { get; set; }
        public CrudEnum ModificationType { get; set; }
    }
}
