using ImagePassAPI.Models.DTOs;
using ImagePassAPI.Repositories.Interfaces;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace ImagePassAPI.Repositories.Implementations
{
    public class SentenseRepository : ISentenseRepository
    {
        private readonly IConfiguration _configuration;
        public SentenseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<SentenseDTO> GenerateSentenceAsync()
        {

            var apiKey = _configuration["OpenAIApiKey"];
            
            var requestUri = _configuration["OpenAIApiDefaultScope"];

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            
            var requestBody = new
            {
                model = "gpt-3.5-turbo-instruct",
                prompt = "Generate a brief, one-sentence description of a random objects like animals, nature, manmade things etc. in no more than five words.",
                max_tokens= 10,
                temperature = 0.7
            };

            var jsonBody = JsonSerializer.Serialize(requestBody);

            var response = await httpClient.PostAsync(requestUri, new StringContent(jsonBody, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseJson = JsonDocument.Parse(responseContent);

                var choices = responseJson.RootElement.GetProperty("choices");

                if (choices.GetArrayLength() > 0)
                {
                    var text = choices[0].GetProperty("text").GetString().Trim().ToLower();

                    text = new string(text.Where(c => !char.IsPunctuation(c)).ToArray());

                    return new SentenseDTO 
                    {
                        sentense = text,
                        httpStatusCode = response.StatusCode
                    };
                }
                else
                {
                    return new SentenseDTO
                    {
                        sentense = "",
                        httpStatusCode = response.StatusCode
                    };
                }
            }
            else
            {
                return new SentenseDTO 
                {
                    sentense = "",
                    httpStatusCode = response.StatusCode
                };
            }
        }
    }
}
