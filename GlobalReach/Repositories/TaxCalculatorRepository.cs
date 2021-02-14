using GlobalReach.Models;
using GlobalReach.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

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

        public async Task<Exchange> CalculateCurrencyExchangeAsync(DateTime invoiceDate, double preTaxAmmount, string currency)
        {
            var fixerResponse = await GetExchangeRateAsync(invoiceDate, new string[] { currency });
            if (fixerResponse.Success)
            {
                double exchRate;
                fixerResponse.Rates.TryGetValue(currency, out exchRate);
                var _preTaxAmmount = preTaxAmmount * exchRate;

                double _taxRate;
                _taxRateOptions.TryGetValue(currency, out _taxRate);
                var _taxAmount = _preTaxAmmount * _taxRate;

                return new Exchange
                {
                    ExchangeRate = exchRate,
                    PreTaxAmount = _preTaxAmmount,
                    TaxAmount = _taxAmount,
                    GrandTotal = _preTaxAmmount + _taxAmount
                };
            }
            return null;
        }

        public async Task<FixerResponse> GetExchangeRateAsync(DateTime invoiceDate, string[] symbols)
        {
            var queryString = QueryHelpers.ParseQuery(string.Empty);
            queryString.Add("access_key", _fixerOptions.ApiKey);
            queryString.Add("symbols", string.Join(",", symbols));

            var uriBuilder = new StringBuilder(_fixerOptions.Uri)
                            .Append(invoiceDate.Date.ToString("yyyy-MM-dd"))
                            .Append(QueryString.Create(queryString).ToString());

            
            using (var httpClient = new HttpClient())
            {
                var streamTask = await httpClient.GetStreamAsync(uriBuilder.ToString());
                return await JsonSerializer.DeserializeAsync<FixerResponse>(streamTask);                
            }
        }
    }
}
