using GlobalReach.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlobalReachTest
{
    public class TaxCalculatorControllerTest : IClassFixture<WebApplicationFactory<GlobalReach.Startup>>
    {
        private readonly HttpClient _httpClient;
        
        public TaxCalculatorControllerTest(WebApplicationFactory<GlobalReach.Startup> appFactory)
        {
            _httpClient = appFactory.CreateClient();
        }

        private async Task<HttpResponseMessage> GetAsync(DateTime invoiceDate, double preTax, string currency)
        {
            var queryString = QueryHelpers.ParseQuery(string.Empty);
            queryString.Add("invoiceDate", invoiceDate.ToString());
            queryString.Add("preTaxAmount", preTax.ToString());
            queryString.Add("currency", currency);

            var uri = new StringBuilder(_httpClient.BaseAddress.ToString())
                            .Append("api/")
                            .Append("taxcalculator")                                                        
                            .Append(QueryString.Create(queryString).ToString())
                            .ToString();

            return await _httpClient.GetAsync(uri);

            //return await _httpClient.GetAsync(_httpClient.BaseAddress 
            //    + $"api/taxcalculator?invoiceDate={invoiceDate}&preTaxAmount={preTax}&currency={currency}");
        }

        [Fact]
        public async Task Get_Should_Retrieve_Calculated_Exchange_USD()
        {
            //Arrange
            var invoiceDate = new DateTime(2020, 08, 05);
            var preTax = 123.45;
            var currency = "USD";

            var expected = new Exchange
            {
                PreTaxAmount = "146.57 USD",
                TaxAmount = "14.66 USD",
                GrandTotal = "161.22 USD",
                ExchangeRate = 1.187247,
            };

            //Action
            var response = await GetAsync(invoiceDate, preTax, currency);
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<Exchange>(await response.Content.ReadAsStringAsync());
            
            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expected.PreTaxAmount, result.PreTaxAmount);
            Assert.Equal(expected.TaxAmount, result.TaxAmount);
            Assert.Equal(expected.GrandTotal, result.GrandTotal);
            Assert.Equal(expected.ExchangeRate, result.ExchangeRate);
        }

        [Fact]
        public async Task Get_Should_Retrieve_Calculated_Exchange_EUR()
        {
            //Arrange
            var invoiceDate = new DateTime(2020, 07, 12);
            var preTax = 1000.00;
            var currency = "EUR";

            var expected = new Exchange
            {
                PreTaxAmount = "1,000.00 EUR",
                TaxAmount = "90.00 EUR",
                GrandTotal = "1,090.00 EUR",
                ExchangeRate = 1,
            };

            //Action
            var response = await GetAsync(invoiceDate, preTax, currency);
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<Exchange>(await response.Content.ReadAsStringAsync());

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expected.PreTaxAmount, result.PreTaxAmount);
            Assert.Equal(expected.TaxAmount, result.TaxAmount);
            Assert.Equal(expected.GrandTotal, result.GrandTotal);
            Assert.Equal(expected.ExchangeRate, result.ExchangeRate);
        }
        
        [Fact]
        public async Task Get_Should_Retrieve_Calculated_Exchange_CAD()
        {
            //Arrange
            var invoiceDate = new DateTime(2020, 08, 19);
            var preTax = 6543.21;
            var currency = "CAD";

            var expected = new Exchange
            {
                PreTaxAmount = "10,239.07 CAD",
                TaxAmount = "1,126.30 CAD",
                GrandTotal = "11,365.37 CAD",
                ExchangeRate = 1.564839,
            };

            //Action
            var response = await GetAsync(invoiceDate, preTax, currency);
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<Exchange>(await response.Content.ReadAsStringAsync());

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expected.PreTaxAmount, result.PreTaxAmount);
            Assert.Equal(expected.TaxAmount, result.TaxAmount);
            Assert.Equal(expected.GrandTotal, result.GrandTotal);
            Assert.Equal(expected.ExchangeRate, result.ExchangeRate);
        }

        [Fact]
        public async Task Get_Should_Retrieve_BadRequest_FutureDate()
        {
            //Arrange
            var invoiceDate = new DateTime(2022, 08, 05);
            var preTax = 123.45;
            var currency = "USD";
            
            //Action
            var response = await GetAsync(invoiceDate, preTax, currency);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Get_Should_Retrieve_BadRequest_NegativePreTax()
        {
            //Arrange
            var invoiceDate = new DateTime(2020, 08, 05);
            var preTax = -123.45;
            var currency = "USD";

            //Action
            var response = await GetAsync(invoiceDate, preTax, currency);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Get_Should_Retrieve_BadRequest_ZeroPreTax()
        {
            //Arrange
            var invoiceDate = new DateTime(2020, 08, 05);
            var preTax = 0;
            var currency = "USD";

            //Action
            var response = await GetAsync(invoiceDate, preTax, currency);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Get_Should_Retrieve_BadRequest_InvalidCurrency()
        {
            //Arrange
            var invoiceDate = new DateTime(2020, 08, 05);
            var preTax = 123.45;
            var currency = "GARBAGE";

            //Action
            var response = await GetAsync(invoiceDate, preTax, currency);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Get_Should_Retrieve_BadRequest_EmptyCurrency()
        {
            //Arrange
            var invoiceDate = new DateTime(2020, 08, 05);
            var preTax = 123.45;
            var currency = string.Empty;

            //Action
            var response = await GetAsync(invoiceDate, preTax, currency);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Get_Should_Retrieve_Exchange_LowerCaseCurrency()
        {
            //Arrange
            var invoiceDate = new DateTime(2020, 08, 05);
            var preTax = 123.45;
            var currency = "usd";

            //Action
            var response = await GetAsync(invoiceDate, preTax, currency);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Get_Should_Retrieve_Exchange_Today()
        {
            //Arrange
            var invoiceDate = DateTime.Today;
            var preTax = 123.45;
            var currency = "USD";

            //Action
            var response = await GetAsync(invoiceDate, preTax, currency);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Get_Should_Retrieve_Exchange_CAD()
        {
            //Arrange
            var invoiceDate = DateTime.Today;
            var preTax = 123.45;
            var currency = "CAD";

            //Action
            var response = await GetAsync(invoiceDate, preTax, currency);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Get_Should_Retrieve_BadRequest_NewDate()
        {
            //Arrange
            DateTime invoiceDate = new DateTime();
            var preTax = 123.45;
            var currency = "CAD";

            //Action
            var response = await GetAsync(invoiceDate, preTax, currency);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Get_Should_Retrieve_Exchange_EUR()
        {
            //Arrange
            var invoiceDate = DateTime.Today;
            var preTax = 123.45;
            var currency = "EUR";

            //Action
            var response = await GetAsync(invoiceDate, preTax, currency);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
