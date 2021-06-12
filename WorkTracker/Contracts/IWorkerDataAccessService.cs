using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WorkTracker.Database.DTOs;

namespace WorkTracker.Contracts
{
    public interface IWorkerDataAccessService
    {
        /// <summary>
        /// Gets all the worker belongs to a owner
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        Task<List<WorkerDTO>> GetAllWorker(int ownerId);

        /// <summary>
        /// Add a new worker 
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="workerName"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        Task<WorkerDTO> AddWorker(int ownerId, string workerName, string mobile);

        /// <summary>
        /// Calculate salary for a worker
        /// </summary>
        /// <param name="workerId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        Task<int> CalculateSalary(int workerId, DateTime startDate, DateTime endDate);
    }
}
