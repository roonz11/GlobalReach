namespace GlobalReach.Models
{
    public class ExchangeResponse
    {
        public Exchange Exchange { get; set; }
        public bool Success { get; set; }
        public string[] Errors { get; set; }
    }
}
