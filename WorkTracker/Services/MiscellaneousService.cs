using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WorkTracker.Contracts;
using WorkTracker.WebAccess.Interfaces;

namespace WorkTracker.Services
{
    public class MiscellaneousService : IMiscellaneousService
    {
        private string _controller = "Miscellaneous";
        private IWebDataAccess _webAccess;

        public MiscellaneousService(IWebDataAccess webDataAccess)
        {
            _webAccess = webDataAccess;
        }
        public async Task<int> GenerateOTPForUser(string userEmail = null)
        {
            return await _webAccess.GetAsync<int>(
                $"{_controller}/GetOtpForUser?email={userEmail}");
        }
    }
}
