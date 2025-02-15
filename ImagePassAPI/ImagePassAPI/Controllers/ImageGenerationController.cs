using ImagePassAPI.Models.DTOs;
using ImagePassAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ImagePassAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageGenerationController : ControllerBase
    {
        private readonly IImageRepository _iImageRepository;
        public ImageGenerationController(IImageRepository imageRepository)
        {
            _iImageRepository = imageRepository;
        }

        [HttpGet("{imageId}")]
        public async Task<IActionResult> Get(string imageId)
        {
            var response = await _iImageRepository.GetImageByImageIdAsync(imageId.ToString());

            if (response == null)
            {
                return NotFound();
            }
            else if (!response.IsImageGenerated)
            {
                return Accepted(response);
            }

            return Ok(response);

        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string prompt)
        {
            var response = await _iImageRepository.GenerateImageAsync(prompt);

            if (response == null)
            {
                return BadRequest();
            }

            return Accepted(response);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
