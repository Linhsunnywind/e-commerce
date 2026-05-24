using E_commerce.Data;
using E_commerce.DTOs.Login;
using E_commerce.Repositories;
using E_commerce.Repositories.Interfaces;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public LoginService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }   

        public async Task<ServiceResponse<LoginResponse>> LoginAsync(LoginRequestDTO dto)
        {
            var response = new ServiceResponse<LoginResponse>();

            var user = await _userRepository.GetByEmailAsync(dto.Email.Trim().ToLower());

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                response.IsSuccess = false;
                response.Message = "Email or password is incorrect.";
                return response;
            }

            response.Data = new LoginResponse
            {
                Email = user.Email,
                Name = user.Name,
                RoleName = user.Role?.Name ?? "Customer",
                Token = _tokenService.CreateToken(user)
            };
            response.IsSuccess = true;
            response.Message = "Login successful.";
            return response;
        }
    }
}
