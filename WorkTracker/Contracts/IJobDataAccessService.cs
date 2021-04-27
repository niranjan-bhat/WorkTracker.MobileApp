using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WorkTracker.Database.DTOs;

namespace WorkTracker.Contracts
{
    public interface IJobDataAccessService
    {
        /// <summary>
        /// Get a job by its Id from server
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        Task<JobDTO> GetJobById(int jobId);

        /// <summary>
        /// Fetches all the jobs belongs to this owner
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        Task<List<JobDTO>> GetAllJob(int ownerId);


        /// <summary>
        /// Insert a new job for this owner
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="jobname"></param>
        /// <returns></returns>
        Task<JobDTO> InsertJob(int ownerId, string jobname);
    }
}
