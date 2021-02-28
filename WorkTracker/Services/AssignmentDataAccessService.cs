using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using WorkTracker.Contracts;
using WorkTracker.Database.DTOs;
using WorkTracker.WebAccess.Interfaces;

namespace WorkTracker.Services
{
    public class AssignmentDataAccessService : IAssignmentDataAccessService
    {
        private string _controller = "Assignment";
        private IWebDataAccess _webAccess;

        public AssignmentDataAccessService(IWebDataAccess webDataAccess)
        {
            _webAccess = webDataAccess;
        }

        public async Task<AssignmentDTO> GetAssignmentById(int id)
        {
            return await _webAccess.GetAsync<AssignmentDTO>($"{_controller}?id={id}");
        }

        public async Task<AssignmentDTO> InsertAssignment(int ownerId, int wage, int workerId, DateTime assignedDate, List<JobDTO> jobs)
        {
            var assignedDateUrlParam =assignedDate.ToString(Constants.DateFormat);
            var stringContent = new StringContent(JsonConvert.SerializeObject(jobs), Encoding.UTF8, "application/json");

            var result = await _webAccess.PostAsync<AssignmentDTO>($"{_controller}?ownerId={ownerId}&wage={wage}&workerId={workerId}&assignedDate={assignedDateUrlParam}", stringContent);
            return result;
        }

        public async Task<List<AssignmentDTO>> GetAllAssignment(int ownerId, DateTime startDate, DateTime enDateTime, int workerId)
        {
            var startDateUrl = HttpUtility.UrlEncode(startDate.ToString(Constants.DateFormat));
            var endDateUrl = HttpUtility.UrlEncode(enDateTime.ToString(Constants.DateFormat));

            return await _webAccess.GetAsync<List<AssignmentDTO>>($"{_controller}/GetAllAssignment?ownerId={ownerId}&startDate={startDateUrl}&endDate={endDateUrl}&workerId={workerId}");
        }

        public async Task<CommentDTO> AddComment(int assignmentId, string comment)
        {
            var comUrl = HttpUtility.UrlEncode(comment);
            return await _webAccess.PostAsync<CommentDTO>($"{_controller}/AddComment?assignmentId={assignmentId}&comment={comUrl}", null);
        }

        public async Task<AssignmentDTO> GetAssignmentOnDate(int workerId, DateTime date, int ownerId)
        {
            var assignedDate = HttpUtility.UrlEncode(date.ToString(Constants.DateFormat));

            return await _webAccess.GetAsync<AssignmentDTO>($"{_controller}/GetAssignmentByDate?workerId={workerId}&assignedDate={assignedDate}&ownerId={ownerId}");

        }
    }
}
