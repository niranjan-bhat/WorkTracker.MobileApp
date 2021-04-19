using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WorkTracker.Contracts;
using WorkTracker.Database.DTO;
using WorkTracker.WebAccess.Interfaces;
using WorkTracker.WebAccessLayer.Interfaces;

namespace WorkTracker.Services
{
    public class OwnerDataAccessService : IOwnerDataAccessService
    {
        private string _controller = "Owner/";
        private IAuthorization _auth;
        private IWebDataAccess _webAccess;
        private IEncryptionHelper _encryptionHelper;


        public OwnerDataAccessService(IWebDataAccess webDataAccess, IAuthorization authorization, IEncryptionHelper encryptionHelper)
        {
            _webAccess = webDataAccess;
            _auth = authorization;
            _encryptionHelper = encryptionHelper;
        }
        public async Task<bool> Login(string email, string password)
        {
            var response = await _auth.AuthorizeUser(email, _encryptionHelper.Encrypt(password));
            return response;
        }

        public async Task<OwnerDTO> Register(string name, string email, string password)
        {
            var encryptedPassword = HttpUtility.UrlEncode(_encryptionHelper.Encrypt(password));
            email = HttpUtility.UrlEncode(email);
            name = HttpUtility.UrlEncode(name);
            return await _webAccess.PostAsync<OwnerDTO>($"{_controller}RegisterUser?name={name}&email={email}&encryptedPassword={encryptedPassword}", null);
        }

        public async Task<OwnerDTO> GetOwnerByEmail(string email)
        {
            var emailUrlEncoded = HttpUtility.UrlEncode(email);
            var response = await _webAccess.GetAsync<OwnerDTO>($"{_controller}GetUserByEmail?email={emailUrlEncoded}");
            return response;
        }

        public async Task<bool> VerifyEmail(string email)
        {
            var emailUrlEncoded = HttpUtility.UrlEncode(email); 
            return await _webAccess.PostAsync<bool>($"{_controller}VerifyEmail?email={emailUrlEncoded}", null);
        }
    }
}
