using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WorkTracker.Contracts
{
    public interface IMiscellaneousService
    {
        /// <summary>
        /// Generates OTP and sends Email to user if userEmail is specified
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        Task<int> GenerateOTPForUser(string userEmail = null);
    }
}
