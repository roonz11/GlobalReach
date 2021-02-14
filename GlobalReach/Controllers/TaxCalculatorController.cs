using GlobalReach.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GlobalReach.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxCalculatorController : ControllerBase
    {
        private readonly ITaxCalculatorRepository _taxCalculatorRepository;

        public TaxCalculatorController(ITaxCalculatorRepository taxCalculatorRepository)
        {
            _taxCalculatorRepository = taxCalculatorRepository;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAsync(DateTime invoiceDate, double preTaxAmount, string currency)
        {
            try
            {
                var result = await _taxCalculatorRepository.CalculateCurrencyExchangeAsync(invoiceDate, preTaxAmount, currency);
                if (result != null)
                    return Ok(result);
                else
                    return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
