using System.Collections.Generic;

namespace GlobalReach.Options
{
    public class FixerOptions
    {
        public string ApiKey { get; set; }
        public string Uri { get; set; }
        public IEnumerable<string> Symbols { get; set; }
    }
}
