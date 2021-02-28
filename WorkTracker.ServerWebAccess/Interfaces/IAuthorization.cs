using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WorkTracker.WebAccess.Interfaces
{
    public interface IAuthorization
    {
        Task<bool> AuthorizeUser(string email, string password);
    }
}
