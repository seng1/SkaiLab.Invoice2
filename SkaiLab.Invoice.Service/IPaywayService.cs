using SkaiLab.Invoice.Models.Payway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public interface IPaywayService : IPaymentService
    {
        Task ProcessPayAsync(RequestComplete requestComplete);
    }
}
