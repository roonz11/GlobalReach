using GlobalReach.Helpers;
using GlobalReach.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GlobalReach.Services
{
    public class TaxCalculatorService : ITaxCalculatorService
    {        
        private readonly Dictionary<string, double> _taxRateOptions;
        private readonly IExchangeRateService _exchangeRateService;

        public TaxCalculatorService(IOptions<Dictionary<string, double>> taxRateOptions,
            IExchangeRateService exchangeRateService)
        {            
            _taxRateOptions = taxRateOptions.Value;
            _exchangeRateService = exchangeRateService;
        }

        public async Task<ExchangeResponse> CalculateCurrencyExchangeAsync(DateTime invoiceDate, double preTaxAmount, string currency)
        {            
            if (invoiceDate > DateTime.Now || invoiceDate == null)
            {
                return new ExchangeResponse
                {
                    Errors = new[] { "Invoice Date is Invalid" }
                };
            }

            if (preTaxAmount <= 0)
            {
                return new ExchangeResponse
                {
                    Errors = new[] { "Pre Tax Amount is Invalid" }
                };
            }

            if (string.IsNullOrEmpty(currency) || !_taxRateOptions.ContainsKey(currency.ToUpper()))
            {
                return new ExchangeResponse
                {
                    Errors = new[] { "Currency is not supported" }
                };
            }

            return await CalculateTaxAsync(invoiceDate, preTaxAmount, currency);
        }

        private async Task<ExchangeResponse> CalculateTaxAsync(DateTime invoiceDate, double preTaxAmount, string currency)
        {
            var fixerResponse = await _exchangeRateService.GetExchangeRateAsync(invoiceDate, new string[] { currency });
            if (!fixerResponse.Success)
            {
                return new ExchangeResponse
                {
                    Errors = new[] { "Could not get conversion rate" }
                };
            }

            double exchRate;
            fixerResponse.Rates.TryGetValue(currency, out exchRate);
            var calcPreTaxAmount = preTaxAmount * exchRate;

            double taxRate;
            _taxRateOptions.TryGetValue(currency, out taxRate);
            var calcTaxAmount = calcPreTaxAmount * (taxRate / 100);

            return new ExchangeResponse
            {
                Exchange = new Exchange
                {
                    PreTaxAmount = CurrencyHelper.DisplayAs(calcPreTaxAmount, currency),
                    TaxAmount = CurrencyHelper.DisplayAs(calcTaxAmount, currency),
                    GrandTotal = CurrencyHelper.DisplayAs(calcPreTaxAmount + calcTaxAmount, currency),
                    ExchangeRate = exchRate,
                },
                Success = true
            };
        }
    }
}
