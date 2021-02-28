using System;
using System.Collections.Generic;
using System.Text;
using WorkTracker.Database.DTO;

namespace WorkTracker.Database.DTOs
{
    public class AssignmentDTO
    {
        public int Id { get; set; }
        public int WorkerId { get; set; }
        public int OwnerId { get; set; }
        public DateTime AssignedDate { get; set; }
        public int? Wage { get; set; }

        public WorkerDTO Worker { get; set; }
        public OwnerDTO Owner { get; set; }
        public ICollection<CommentDTO> Comments { get; set; }
        public ICollection<JobDTO> Jobs { get; set; }
    }
}
