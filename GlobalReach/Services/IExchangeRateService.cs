using GlobalReach.Models;
using System;
using System.Threading.Tasks;

namespace GlobalReach.Services
{
    public interface IExchangeRateService
    {
        Task<FixerResponse> GetExchangeRateAsync(DateTime invoiceDate, string[] symbols);
    }
}
