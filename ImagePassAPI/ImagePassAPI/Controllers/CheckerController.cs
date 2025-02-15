
using Microsoft.AspNetCore.Mvc;

namespace ImagePassAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckerController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public CheckerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("{emailToCheck}")]
        public async Task<IActionResult> GetEmailChecked(string emailToCheck)
        {
            using (HttpClient httpClient = new HttpClient()) 
            {
                httpClient.DefaultRequestHeaders.Add("hibp-api-key", _configuration["HaveIBeenPwnedAPIKey"]);
                httpClient.DefaultRequestHeaders.Add("User-Agent", "ImagePassAPI"); 

                var response = await httpClient.GetAsync(_configuration["HaveIBeenPwnedDefalutScope"] + emailToCheck + "?truncateResponse=false");

                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        var result = await response.Content.ReadAsStringAsync();
                        return Ok(result);
                    case System.Net.HttpStatusCode.NotFound:
                        return NotFound("No breaches found for " + emailToCheck);
                    case System.Net.HttpStatusCode.TooManyRequests:
                        return StatusCode(429, "Too many requests. Please try again later.");
                    case System.Net.HttpStatusCode.BadRequest:
                        return BadRequest("Invalid email address.");
                    case System.Net.HttpStatusCode.Forbidden:
                        return StatusCode(403, "Forbidden — no user agent has been specified in the request");
                    case System.Net.HttpStatusCode.Unauthorized:
                        return StatusCode(401, "Unauthorized — the API key has been revoked, or the Authorization header is missing from the request");
                    case System.Net.HttpStatusCode.ServiceUnavailable:
                        return StatusCode(503, "Service Unavailable — the API temporarily overloaded");
                    default:
                        return StatusCode(500, "External API temporarily unavailable");
                }
            }

        }

    }
}
