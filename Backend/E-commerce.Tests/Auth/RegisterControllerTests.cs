using E_commerce.Controllers.AuthController;
using E_commerce.DTOs.Auth;
using E_commerce.Services;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace E_commerce.Tests.Auth
{
    public class RegisterControllerTests
    {
        [Fact]
        public async Task Register_ReturnsOk_WhenRegistrationSucceeds()
        {
            var service = new Mock<IRegisterService>();
            service.Setup(s => s.RegisterAsync(It.IsAny<RegisterRequest>()))
                .ReturnsAsync(new ServiceResponse<UserInfoResponse>
                {
                    IsSuccess = true,
                    Message = "Registered successfully.",
                    Data = new UserInfoResponse { Name = "Nguyen Van A", Email = "vana@example.com" }
                });

            var controller = new RegisterController(service.Object);
            var result = await controller.Register(new RegisterRequest
            {
                Name = "Nguyen Van A",
                Email = "vana@example.com",
                PhoneNumber = "0901234567",
                Password = "Pass@123"
            });

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<UserInfoResponse>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Equal("vana@example.com", payload.Data?.Email);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenEmailAlreadyExists()
        {
            var service = new Mock<IRegisterService>();
            service.Setup(s => s.RegisterAsync(It.IsAny<RegisterRequest>()))
                .ReturnsAsync(new ServiceResponse<UserInfoResponse>
                {
                    IsSuccess = false,
                    Message = "Email already exists."
                });

            var controller = new RegisterController(service.Object);
            var result = await controller.Register(new RegisterRequest
            {
                Name = "Nguyen Van A",
                Email = "dup@example.com",
                PhoneNumber = "0901234567",
                Password = "Pass@123"
            });

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<UserInfoResponse>>(bad.Value);
            Assert.False(payload.Success);
        }
    }
}
