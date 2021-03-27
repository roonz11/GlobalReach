using GlobalReach.Models;
using GlobalReach.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GlobalReach.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly FixerOptions _fixerOptions;

        public ExchangeRateService(IOptions<FixerOptions> fixerOptions)
        {
            _fixerOptions = fixerOptions.Value;
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
