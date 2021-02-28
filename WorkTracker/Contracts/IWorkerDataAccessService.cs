using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WorkTracker.Database.DTOs;

namespace WorkTracker.Contracts
{
    public interface IWorkerDataAccessService
    {
        Task<List<WorkerDTO>> GetAllWorker(int ownerId);
        Task<WorkerDTO> AddWorker(int ownerId, string workerName, string mobile);
    }
}
