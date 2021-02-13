using System;
using System.Collections.Generic;

namespace GlobalReach.Models
{
    public class FixerResponse
    {
        public bool Success { get; set; }
        public double Timestamp { get; set; }
        public string Base { get; set; }
        public DateTime Date { get; set; }
        public IEnumerable<Rate> Rates { get; set; }        
    }

    public class Rate
    {
        public string Symbol { get; set; }
        public double Value { get; set; }
    }
}
