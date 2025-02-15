namespace ImagePassAPI.Repositories.Interfaces
{
    public interface IPasswordRepository
    {
        public Task<string> GeneratePasswordAsync(string sentense);
    }
}
