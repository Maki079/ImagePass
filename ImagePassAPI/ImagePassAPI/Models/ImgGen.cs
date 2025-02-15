using MongoDB.Bson;

namespace ImagePassAPI.Models
{
    public class ImgGen
    {
        public ObjectId Id { get; set; } // MongoDB unique identifier
        private string _imageEncodedInBase64;
        public string ImageEncodedInBase64 
        {
            get => _imageEncodedInBase64;
            set 
            {
                if (String.IsNullOrEmpty(value)) 
                {
                    _imageEncodedInBase64 = String.Empty;
                }
                else
                {
                    _imageEncodedInBase64 = value;
                }
                
            }
        }
    }
}
