using System;
using System.Collections.Generic;
using System.Text;
using WorkTracker.Database.DTO;

namespace WorkTracker.Database.DTOs
{
    public class JobDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OwnerId { get; set; }
    }
}
