using System.Net;

namespace ImagePassAPI.Models.DTOs
{
    public class SentenseDTO
    {
        public string? sentense { get; set; }
        public HttpStatusCode httpStatusCode { get; set; }
    }
}
