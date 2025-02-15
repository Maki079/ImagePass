using ImagePassAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ImagePassAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordGenerationController : ControllerBase
    {
        private readonly IPasswordRepository _passwordRepository;
        public PasswordGenerationController(IPasswordRepository passwordRepository)
        {
            _passwordRepository = passwordRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string sentense)
        {
            var response = await _passwordRepository.GeneratePasswordAsync(sentense);

            return Ok(response);
        }
    }
}
