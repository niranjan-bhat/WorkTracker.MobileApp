using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WorkTracker.DataAccess.DTOs;
using WorkTracker.ViewModels;

namespace WorkTracker.Contracts
{
    public interface IPaymentService
    {
        /// <summary>
        /// Retrieves the payments made by/to worker
        /// </summary>
        /// <param name="workerId"></param>
        /// <returns></returns>
        Task<IEnumerable<PaymentDTO>> GetAllPayments(int workerId);

        /// <summary>
        /// Insert a payment for a worker
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        Task<PaymentDTO> AddPayment(PaymentDTO payment);
    }
}
