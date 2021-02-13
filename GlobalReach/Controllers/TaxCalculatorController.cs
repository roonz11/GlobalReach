using GlobalReach.Models;
using GlobalReach.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        // GET: api/<TaxCalculatorController>
        [HttpGet]
        public async Task<ExchangeDTO> Get(DateTime invoiceDate, double preTaxAmmount, string currency)
        {
            var result = await _taxCalculatorRepository.CalculateCurrencyExchangeAsync(invoiceDate, preTaxAmmount, currency);
            return result;
        }

        // GET api/<TaxCalculatorController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<TaxCalculatorController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<TaxCalculatorController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TaxCalculatorController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
