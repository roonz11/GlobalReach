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

namespace GlobalReach.Repositories
{
    public class TaxCalculatorRepository : ITaxCalculatorRepository
    {
        private readonly FixerOptions _fixerOptions;
        private readonly Dictionary<string, double> _taxRateOptions;

        public TaxCalculatorRepository(IOptions<FixerOptions> fixerOptions,
            IOptions<Dictionary<string, double>> taxRateOptions)
        {
            _fixerOptions = fixerOptions.Value;
            _taxRateOptions = taxRateOptions.Value;
        }

        public async Task<Exchange> CalculateCurrencyExchangeAsync(DateTime invoiceDate, double preTaxAmount, string currency)
        {
            if (!ValidInputs(invoiceDate, preTaxAmount, currency))
                return null;

            var fixerResponse = await GetExchangeRateAsync(invoiceDate, new string[] { currency });
            if (fixerResponse.Success)
            {
                double exchRate;
                fixerResponse.Rates.TryGetValue(currency, out exchRate);
                var calcPreTaxAmount = preTaxAmount * exchRate;

                double taxRate;
                _taxRateOptions.TryGetValue(currency, out taxRate);
                var calcTaxAmount = calcPreTaxAmount * (taxRate/100);

                return new Exchange
                {
                    PreTaxAmount = CurrencyHelper.DisplayAs(calcPreTaxAmount, currency),
                    TaxAmount = CurrencyHelper.DisplayAs(calcTaxAmount, currency),
                    GrandTotal = CurrencyHelper.DisplayAs(calcPreTaxAmount + calcTaxAmount, currency),
                    ExchangeRate = exchRate
                };
            }

            return null;
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

        private bool ValidInputs(DateTime invoiceDate, double preTaxAmount, string currency)
        {
            if (invoiceDate > DateTime.Now || invoiceDate == null)
                return false;
            if (preTaxAmount <= 0)
                return false;
            if (string.IsNullOrEmpty(currency) || !_taxRateOptions.ContainsKey(currency.ToUpper()))
                return false;

            return true;
        }
    }
}
