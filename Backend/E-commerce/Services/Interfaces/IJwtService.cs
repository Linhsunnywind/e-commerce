using E_commerce.Models;

namespace E_commerce.Services.Interfaces
{
    public interface IJwtService
    {
        public string CreateToken(User user);
    }
}
