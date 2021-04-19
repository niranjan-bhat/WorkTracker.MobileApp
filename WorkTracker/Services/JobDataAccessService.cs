using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WorkTracker.Contracts;
using WorkTracker.Database.DTOs;
using WorkTracker.WebAccessLayer.Interfaces;

namespace WorkTracker.Services
{
    public class JobDataAccessService : IJobDataAccessService
    {
        private string _controller = "Job";
        private IWebDataAccess _webAccess;

        public JobDataAccessService(IWebDataAccess webDataAccess)
        {
            _webAccess = webDataAccess;
        }
        public async Task<JobDTO> GetJobById(int jobId)
        {
            return await _webAccess.GetAsync<JobDTO>($"{_controller}/GetJobById?id={jobId}");
        }

        public async Task<List<JobDTO>> GetAllJob(int ownerId)
        {
            return await _webAccess.GetAsync<List<JobDTO>>($"{_controller}/GetAllJobsForOwner?ownerId={ownerId}");
        }

        public async Task<JobDTO> InsertJob(int ownerId, string jobname)
        {
            return await _webAccess.PostAsync<JobDTO>($"{_controller}?ownerId={ownerId}&jobName={jobname}", null);
        }
    }
}
