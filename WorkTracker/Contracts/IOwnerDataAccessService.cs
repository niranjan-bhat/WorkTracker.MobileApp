using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WorkTracker.Database.DTO;

namespace WorkTracker.Contracts
{
    public interface IOwnerDataAccessService
    {
        /// <summary>
        /// Checkes if user is a registered user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<bool> Login(string email, string password);
        
        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<OwnerDTO> Register(string name, string email, string password);

        /// <summary>
        /// Gets a user by his email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<OwnerDTO> GetOwnerByEmail(string email);

        /// <summary>
        /// Verify user's email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<bool> VerifyEmail(string email);
    }
}
