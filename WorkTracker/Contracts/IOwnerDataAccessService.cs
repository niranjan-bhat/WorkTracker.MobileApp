using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WorkTracker.Database.DTO;

namespace WorkTracker.Contracts
{
    public interface IOwnerDataAccessService
    {
        Task<bool> Login(string email, string password);
        Task<OwnerDTO> Register(string name, string email, string password);
        Task<OwnerDTO> GetOwnerByEmail(string email);
    }
}
