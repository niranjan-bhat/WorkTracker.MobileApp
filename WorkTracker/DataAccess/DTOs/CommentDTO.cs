using System;
using System.Collections.Generic;
using System.Text;

namespace WorkTracker.Database.DTOs
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public string OwnerComment { get; set; }
        public int AssignmentId { get; set; }
    }
}
