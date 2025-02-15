using ImagePassAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ImagePassAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SentesnseGenerationController : ControllerBase
    {
        private readonly ISentenseRepository _iSentenseRepository;
        public SentesnseGenerationController(ISentenseRepository sentenseRepository)
        {
            _iSentenseRepository = sentenseRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _iSentenseRepository.GenerateSentenceAsync();

            switch (response.httpStatusCode)
            {
                case HttpStatusCode.OK:
                    return Ok(response.sentense);
                case HttpStatusCode.Unauthorized:
                    return Unauthorized();
                case HttpStatusCode.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode(500, "Ineternal Error");
            }
        }
    }
}
