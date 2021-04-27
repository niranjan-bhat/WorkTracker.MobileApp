using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WorkTracker.Database.DTOs;

namespace WorkTracker.Contracts
{
    public interface IAssignmentDataAccessService
    {
        /// <summary>
        /// Fetches the assignment from the server using the ID 
        /// </summary>
        /// <param name="id">Id of the assignment</param>
        /// <returns></returns>
        Task<AssignmentDTO> GetAssignmentById(int id);

        /// <summary>
        /// Add a new assignment
        /// </summary>
        /// <param name="ownerId">Oner of the assignment</param>
        /// <param name="wage">Wage for this worker for this assignment</param>
        /// <param name="workerId">Worker involved in this assignment</param>
        /// <param name="assignedDate">Date of the assignment</param>
        /// <param name="jobs">Collection of jobs assignment</param>
        /// <returns></returns>
        Task<AssignmentDTO> InsertAssignment(int ownerId, int wage, int workerId, DateTime assignedDate,
            List<JobDTO> jobs);

        /// <summary>
        /// Fetches all the assignment for a given date range and owner and worker
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="startDate"></param>
        /// <param name="enDateTime"></param>
        /// <param name="workerId"></param>
        /// <returns></returns>
        Task<List<AssignmentDTO>> GetAllAssignment(int ownerId, DateTime startDate, DateTime enDateTime, int? workerId);

        /// <summary>
        /// Adds a comment for an assignment
        /// </summary>
        /// <param name="assignmentId"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        Task<CommentDTO> AddComment(int assignmentId, string comment);

        /// <summary>
        /// Deletes an assignment
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="assignedDate"></param>
        /// <returns></returns>
        Task<bool> DeleteAssignments(int ownerId, DateTime assignedDate);
    }
}
