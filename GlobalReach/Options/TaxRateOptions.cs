using GlobalReach.Models;
using System.Collections.Generic;

namespace GlobalReach.Options
{
    public class TaxRateOptions
    {
        public IEnumerable<Rate> TaxRates { get; set; }
    }
}
