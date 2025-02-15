namespace ImagePassAPI.Models.DTOs
{
    public class SDApiRequestDTO
    {
        public string prompt { get; set; }
        public int steps { get; set; }
        public string imageId { get; set; }
    }
}
