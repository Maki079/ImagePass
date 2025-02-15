using ImagePassAPI.Models.DTOs;

namespace ImagePassAPI.Repositories.Interfaces
{
    public interface IImageRepository
    {
        public Task<ImageDTO> GenerateImageAsync(string prompt);
        public Task<ImageDTO?> GetImageByImageIdAsync(string imageId);
    }
}
