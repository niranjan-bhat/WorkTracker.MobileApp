using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using WorkTracker.Database;
using WorkTracker.Database.DTOs;

namespace WorkTracker.Classes
{
    public class UiBindableAssignment : SelectableItem
    {
        public AssignmentDTO Assignment { get; set; }

        public ObservableCollection<JobDTO> AssignedJobs { get; set; }
    }
}
