using ImagePassAPI.Code;
using ImagePassAPI.Models;
using ImagePassAPI.Models.DTOs;
using ImagePassAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.Routing;
using MongoDB.Driver;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace ImagePassAPI.Repositories.Implementations
{
    public class ImageRepository : IImageRepository
    {
        private readonly IConfiguration _configuration;
        public ImageRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<ImageDTO> GenerateImageAsync(string prompt)
        {
            prompt = prompt.ToLower();
            prompt = new string(prompt.Where(c => !char.IsPunctuation(c)).ToArray());
            var requestBody = new SDApiRequestDTO
            {
                prompt = prompt,
                steps = 50
            };

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var requestUri = _configuration["SDCapiDefaultScope"];

                // Save request and image in the database
                var imgGen = new ImgGen
                {
                    ImageEncodedInBase64 = "",
                };

                var client = new MongoClient(_configuration["MongoDBConnectionString"]);

                var database = client.GetDatabase(_configuration["MongoDBDatabaseName"]);

                var collection = database.GetCollection<ImgGen>(_configuration["MongoDBCollectionName"]);

                await collection.InsertOneAsync(imgGen);

                var imgGenId = imgGen.Id;

                requestBody.imageId = imgGenId.ToString();

                // setting Content-Type 
                var httpContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                // make the SDAPI call

                var response = await httpClient.PostAsync(requestUri, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    return new ImageDTO
                    {
                        ImageId = imgGenId.ToString(),
                        ImageEncodedInBase64 = "",
                        IsImageGenerated = false
                        
                    };
                }
                else
                {
                    return new ImageDTO
                    {
                        ImageId = "",
                        ImageEncodedInBase64 = "",
                        IsImageGenerated = false
                    };
                }
            }
        }

        public async Task<ImageDTO?> GetImageByImageIdAsync(string imageId)
        {
            var client = new MongoClient(_configuration["MongoDBConnectionString"]);

            var database = client.GetDatabase(_configuration["MongoDBDatabaseName"]);

            var collection = database.GetCollection<ImgGen>(_configuration["MongoDBCollectionName"]);

            var imgGen = await collection.Find<ImgGen>(img => img.Id.ToString() == imageId).FirstOrDefaultAsync();

            if (imgGen == null)
            {
                return null;
            }

            var isImageGenerated = !String.IsNullOrEmpty(imgGen.ImageEncodedInBase64);

            return new ImageDTO
            {
                ImageId = imgGen.Id.ToString(),
                ImageEncodedInBase64 = imgGen.ImageEncodedInBase64,
                IsImageGenerated = isImageGenerated
            };
        }



    }
}
