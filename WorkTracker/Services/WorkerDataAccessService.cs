using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WorkTracker.Contracts;
using WorkTracker.Database.DTOs;
using WorkTracker.WebAccess.Interfaces;
using WorkTracker.WebAccessLayer.Interfaces;

namespace WorkTracker.Services
{
    public class WorkerDataAccessService : IWorkerDataAccessService
    {
        private string _controller = "Worker";
        private IWebDataAccess _webAccess;

        public WorkerDataAccessService(IWebDataAccess webDataAccess)
        {
            _webAccess = webDataAccess;
        }
        public async Task<List<WorkerDTO>> GetAllWorker(int ownerId)
        {
            return await _webAccess.GetAsync<List<WorkerDTO>>($"{_controller}/GetAllWorkersForOwner?ownerId=" + ownerId);
        }

        public async Task<WorkerDTO> AddWorker(int ownerId, string workerName, string mobile)
        {
            return await _webAccess.PostAsync<WorkerDTO>(
                $"{_controller}?ownerId={ownerId}&workerName={workerName}&mobileNumber={mobile}", null);
        }
    }
}
