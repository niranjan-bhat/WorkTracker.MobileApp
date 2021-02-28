using System;
using System.Collections.Generic;
using System.Text;

namespace WorkTracker.Database.DTOs
{
    public class WorkerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public int OwnerId { get; set; }
    }
}
