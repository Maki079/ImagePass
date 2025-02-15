namespace ImagePassAPI.Models.DTOs
{
    public class ImageDTO
    {
        public string ImageId { get; set; }
        public string? ImageEncodedInBase64 { get; set; }
        public bool IsImageGenerated { get; set; }
    }
}
