using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WorkTracker.Contracts;
using WorkTracker.DataAccess.DTOs;
using WorkTracker.ViewModels;
using WorkTracker.WebAccessLayer.Interfaces;

namespace WorkTracker.Services
{
    public class PaymentDataAccessService : IPaymentService
    {
        private string _controller = "Payment";
        private IWebDataAccess _webAccess;

        public PaymentDataAccessService(IWebDataAccess webDataAccess)
        {
            _webAccess = webDataAccess;
        }

        public async Task<IEnumerable<PaymentDTO>> GetAllPayments(int workerId)
        {
            return await _webAccess.GetAsync<IEnumerable<PaymentDTO>>($"{_controller}?workerId={workerId}");
        }

        public async Task<PaymentDTO> AddPayment(PaymentDTO payment)
        {
            var stringContent = new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, "application/json");
            return await _webAccess.PostAsync<PaymentDTO>($"{_controller}", stringContent);
        }
    }
}
