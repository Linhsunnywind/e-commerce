using System.Threading.Tasks;
using E_commerce.Controllers.LoginController;
using E_commerce.DTOs.Login;
using E_commerce.Helpers;
using E_commerce.Services;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace E_commerce.Tests.Auth
{
    public class LoginControllerTests
    {
        [Fact]
        public async Task Login_ReturnsOk_WhenCredentialsValid()
        {
            var loginService = new Mock<ILoginService>();
            var dto = new LoginRequestDTO { Email = "user@example.com", Password = "Pass123!" };
            var response = new ServiceResponse<LoginResponse>
            {
                IsSuccess = true,
                Message = "Login successful.",
                Data = new LoginResponse
                {
                    Email = dto.Email,
                    RoleName = "Customer",
                    Token = "jwt-token"
                }
            };

            loginService.Setup(s => s.LoginAsync(It.IsAny<LoginRequestDTO>()))
                .ReturnsAsync(response);

            var controller = new LoginController(loginService.Object);

            var result = await controller.Login(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
            var payload = Assert.IsType<BaseResponse<LoginResponse>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Equal("jwt-token", payload.Data?.Token);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenCredentialsInvalid()
        {
            var loginService = new Mock<ILoginService>();
            var dto = new LoginRequestDTO { Email = "user@example.com", Password = "Wrong123!" };
            var response = new ServiceResponse<LoginResponse>
            {
                IsSuccess = false,
                Message = "Email or password is incorrect."
            };

            loginService.Setup(s => s.LoginAsync(It.IsAny<LoginRequestDTO>()))
                .ReturnsAsync(response);

            var controller = new LoginController(loginService.Object);

            var result = await controller.Login(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
            var payload = Assert.IsType<BaseResponse<LoginResponse>>(badRequest.Value);
            Assert.False(payload.Success);
        }
    }
}
