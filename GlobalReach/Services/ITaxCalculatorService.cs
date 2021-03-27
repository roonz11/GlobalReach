using GlobalReach.Models;
using System;
using System.Threading.Tasks;

namespace GlobalReach.Services
{
    public interface ITaxCalculatorService
    {
        Task<ExchangeResponse> CalculateCurrencyExchangeAsync(DateTime invoiceDate, double preTaxAmount, string currency);        
    }
}
