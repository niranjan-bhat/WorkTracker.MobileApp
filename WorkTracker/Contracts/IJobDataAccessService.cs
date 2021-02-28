using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WorkTracker.Database.DTOs;

namespace WorkTracker.Contracts
{
    public interface IJobDataAccessService
    {
        Task<JobDTO> GetJobById(int jobId);
        Task<List<JobDTO>> GetAllJob(int ownerId);

        Task<JobDTO> InsertJob(int ownerId, string jobname);
    }
}
