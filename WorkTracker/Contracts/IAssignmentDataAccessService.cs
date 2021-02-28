using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WorkTracker.Database.DTOs;

namespace WorkTracker.Contracts
{
    public interface IAssignmentDataAccessService
    {
        Task<AssignmentDTO> GetAssignmentById(int id);
        Task<AssignmentDTO> InsertAssignment(int ownerId, int wage, int workerId, DateTime assignedDate,
            List<JobDTO> jobs);
        Task<List<AssignmentDTO>> GetAllAssignment(int ownerId, DateTime startDate, DateTime enDateTime, int workerId);
        Task<CommentDTO> AddComment(int assignmentId, string comment);
        Task<AssignmentDTO> GetAssignmentOnDate(int workerId, DateTime date, int ownerId);
    }
}
