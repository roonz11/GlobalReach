using GlobalReach.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GlobalReach.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxCalculatorController : ControllerBase
    {
        private readonly ITaxCalculatorService _taxCalculatorService;

        public TaxCalculatorController(ITaxCalculatorService taxCalculatorService)
        {
            _taxCalculatorService = taxCalculatorService;
        }
        
        /// <summary>
        /// Calculates Exchange rate from EUR to either USD or CAN
        /// 
        /// Assuming all inputs are in EUR -- given the 3 input constraint I was considering using a string
        /// for preTaxAount and splitting the value from the currency, then validating that both amount and currency
        /// I would prefer adding another parameter "fromCurrency" to avoid parsing
        /// </summary>
        /// <param name="invoiceDate"></param>
        /// <param name="preTaxAmount"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAsync(DateTime invoiceDate, double preTaxAmount, string currency)
        {
            try
            {
                var result = await _taxCalculatorService.CalculateCurrencyExchangeAsync(invoiceDate, preTaxAmount, currency);
                if (result.Success)
                    return Ok(result.Exchange);
                else
                    return BadRequest(result.Errors);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
