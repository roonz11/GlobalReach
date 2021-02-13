using GlobalReach.Models;
using GlobalReach.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GlobalReach.Repositories
{
    public class TaxCalculatorRepository : ITaxCalculatorRepository
    {
        private readonly FixerOptions _fixerOptions;
        private readonly TaxRateOptions _taxRateOptions;

        public TaxCalculatorRepository(IOptions<FixerOptions> fixerOptions,
            IOptions<TaxRateOptions> taxRateOptions)
        {
            _fixerOptions = fixerOptions.Value;
            _taxRateOptions = taxRateOptions.Value;
        }

        public async Task<ExchangeDTO> CalculateCurrencyExchangeAsync(DateTime invoiceDate, double preTaxAmmount, string currency)
        {
            var fixerResponse = await GetExchangeRateAsync(invoiceDate, new string[] { currency });
            if (fixerResponse.Success)
            {
                var exchRate = fixerResponse.Rates.SingleOrDefault(x => x.Symbol == currency)?.Value;
                if (exchRate != null)
                {
                    var _preTaxAmmount = preTaxAmmount * exchRate.Value;
                    var _taxAmmount = _preTaxAmmount * _taxRateOptions.TaxRates.SingleOrDefault(x => x.Symbol == currency)?.Value;
                    return new ExchangeDTO
                    {
                        ExchangeRate = exchRate.Value,
                        PreTaxAmount = _preTaxAmmount,
                        TaxAmount = _taxAmmount.Value,
                        GrandTotal = _preTaxAmmount + _taxAmmount.Value
                    };
                }
            }
            return null;
        }

        public async Task<FixerResponse> GetExchangeRateAsync(DateTime invoiceDate, string[] symbols)
        {
            var uriBuidler = new UriBuilder(new Uri(_fixerOptions.Uri));
            uriBuidler.Path = invoiceDate.Date.ToString("yyyy-MM-dd");

            var query = HttpUtility.ParseQueryString(uriBuidler.Query);
            query["access_key"] = _fixerOptions.ApiKey;
            query["symbols"] = string.Join(",", symbols);
            uriBuidler.Query = query.ToString();

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(uriBuidler.ToString());
                using (var urlStream = await response.Content.ReadAsStreamAsync())
                {
                    if (response.IsSuccessStatusCode)
                    {
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(FixerResponse));
                        var fixerResponse = serializer.ReadObject(urlStream) as FixerResponse;
                        return fixerResponse;
                    }
                    else
                        throw new Exception("Failed to get exchange rate");
                }
            }


        }
    }
}
