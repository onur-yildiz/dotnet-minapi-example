namespace Models
{
    public class ResponseContent
    {
        public string? ResultCode { get; set; }
        public string? ResultDescription { get; set; }
        public ResponseBody? Body { get; set; }
        public object? Statistics { get; set; }
    }
}