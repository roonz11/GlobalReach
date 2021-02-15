using GlobalReach.Helpers;
using GlobalReach.Models;
using GlobalReach.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GlobalReach.Services
{
    public class TaxCalculatorService : ITaxCalculatorService
    {
        private readonly FixerOptions _fixerOptions;
        private readonly Dictionary<string, double> _taxRateOptions;

        public TaxCalculatorService(IOptions<FixerOptions> fixerOptions,
            IOptions<Dictionary<string, double>> taxRateOptions)
        {
            _fixerOptions = fixerOptions.Value;
            _taxRateOptions = taxRateOptions.Value;
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
            var fixerResponse = await GetExchangeRateAsync(invoiceDate, new string[] { currency });
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

        public async Task<FixerResponse> GetExchangeRateAsync(DateTime invoiceDate, string[] symbols)
        {
            var queryString = QueryHelpers.ParseQuery(string.Empty);
            queryString.Add("access_key", _fixerOptions.ApiKey);
            queryString.Add("symbols", string.Join(",", symbols));

            var uri = new StringBuilder(_fixerOptions.Uri)
                            .Append(invoiceDate.Date.ToString("yyyy-MM-dd"))
                            .Append(QueryString.Create(queryString).ToString())
                            .ToString();

            using (var httpClient = new HttpClient())
            {
                var streamTask = await httpClient.GetStreamAsync(uri);
                return await JsonSerializer.DeserializeAsync<FixerResponse>(streamTask);
            }
        }

    }
}
