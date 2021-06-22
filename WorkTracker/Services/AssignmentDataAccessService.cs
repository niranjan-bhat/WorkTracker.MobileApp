using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WorkTracker.Contracts;
using WorkTracker.Database.DTOs;
using WorkTracker.WebAccess.Interfaces;
using WorkTracker.WebAccessLayer.Interfaces;

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

        public async Task<List<AssignmentDTO>> InsertAssignment(List<AssignmentDTO> assignmests)
        {
            var stringContent = new StringContent(JsonConvert.SerializeObject(assignmests), Encoding.UTF8, "application/json");

            var result = await _webAccess.PostAsync<List<AssignmentDTO>>($"{_controller}", stringContent);
            return result;
        }

        public async Task<List<AssignmentDTO>> GetAllAssignment(int ownerId, DateTime startDate, DateTime enDateTime, int? workerId, int? jobId = null)
        {
            var startDateUrl = HttpUtility.UrlEncode(startDate.ToString(Constants.DateFormat));
            var endDateUrl = HttpUtility.UrlEncode(enDateTime.ToString(Constants.DateFormat));
            StringBuilder otherParams = new StringBuilder();
            if (workerId.HasValue)
            {
                otherParams.Append($"&workerId={workerId.Value}");
            }
            if (jobId.HasValue)
            {
                otherParams.Append($"&jobId={jobId.Value}");
            }
            return await _webAccess.GetAsync<List<AssignmentDTO>>($"{_controller}/GetAllAssignment?ownerId={ownerId}&startDate={startDateUrl}&endDate={endDateUrl}{otherParams}");
        }

        public async Task<CommentDTO> AddComment(int assignmentId, string comment)
        {
            var comUrl = HttpUtility.UrlEncode(comment);
            return await _webAccess.PostAsync<CommentDTO>($"{_controller}/AddComment?assignmentId={assignmentId}&comment={comUrl}", null);
        }

        public async Task<bool> DeleteAssignments(int ownerId, DateTime assignedDate)
        {
            var assignedDateUrlParam = assignedDate.ToString(Constants.DateFormat);
            return await _webAccess.DeleteAsync<bool>($"{_controller}?ownerId={ownerId}&assignedDate={assignedDateUrlParam}");
        }
    }
}
