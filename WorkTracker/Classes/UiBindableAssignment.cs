using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using WorkTracker.Database;
using WorkTracker.Database.DTOs;

namespace WorkTracker.Classes
{
    public class UiBindableAssignment : SelectableItem
    {
        public AssignmentDTO Assignment { get; set; }

        public ObservableCollection<JobDTO> AssignedJobs { get; set; }

        private int? _wage;
        public int? Wage
        {
            get => _wage;
            set
            {
                _wage = value;
                OnPropertyChanged(nameof(Wage));
            }
        }

        private bool _isAttendanceSubmitted;
        public bool IsAttendanceSubmitted
        {
            get => _isAttendanceSubmitted;
            set
            {
                _isAttendanceSubmitted = value;
                OnPropertyChanged(nameof(IsAttendanceSubmitted));
            }
        }

    }
}
