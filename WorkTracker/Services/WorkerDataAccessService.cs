using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
            var watch = new System.Diagnostics.Stopwatch();

            watch.Start();
            var a= await _webAccess.GetAsync<List<WorkerDTO>>($"{_controller}/GetAllWorkersForOwner?ownerId=" + ownerId);
            watch.Stop();

            Debug.WriteLine("Time now:{0}, elapsed {1}",watch.Elapsed,DateTime.Now);
            return a;
        }

        public async Task<WorkerDTO> AddWorker(int ownerId, string workerName, string mobile)
        {
            return await _webAccess.PostAsync<WorkerDTO>(
                $"{_controller}?ownerId={ownerId}&workerName={workerName}&mobileNumber={mobile}", null);
        }

        public async Task<int> CalculateSalary(int workerId, DateTime startDate, DateTime endDate)
        {
            var startDateUrl = HttpUtility.UrlEncode(startDate.ToString(Constants.DateFormat));
            var endDateUrl = HttpUtility.UrlEncode(endDate.ToString(Constants.DateFormat));

            return await _webAccess.GetAsync<int>(
                $"{_controller}/GetSalary?workerId={workerId}&startingDate={startDateUrl}&endingDate={endDateUrl}");
        }
    }
}
