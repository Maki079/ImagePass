using ImagePassAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ImagePassAPI.Repositories.Interfaces
{
    public interface ISentenseRepository 
    {
        public Task<SentenseDTO> GenerateSentenceAsync();
    }
}
