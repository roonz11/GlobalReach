using GlobalReach.Models;
using System;
using System.Threading.Tasks;

namespace GlobalReach.Repositories
{
    public interface ITaxCalculatorRepository
    {
        Task<ExchangeDTO> CalculateCurrencyExchangeAsync(DateTime invoiceDate, double preTaxAmmount, string currency);
        Task<FixerResponse> GetExchangeRateAsync(DateTime invoiceDate, string[] symbols);
    }
}
