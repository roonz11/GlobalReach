using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GlobalReach.Models
{
    public class FixerResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("timestamp")]
        public double Timestamp { get; set; }

        [JsonPropertyName("historical")]
        public bool Historical { get; set; }

        [JsonPropertyName("base")]
        public string Base { get; set; }
        
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
        
        [JsonPropertyName("rates")]
        public IDictionary<string,double> Rates { get; set; }        
    }
}
