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
        public async Task<IActionResult> Get(DateTime invoiceDate, double preTaxAmmount, string currency)
        {
            try
            {
                var result = await _taxCalculatorRepository.CalculateCurrencyExchangeAsync(invoiceDate, preTaxAmmount, currency);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
