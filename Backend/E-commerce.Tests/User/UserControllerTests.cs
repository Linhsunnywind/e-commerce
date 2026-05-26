using System;
using System.Security.Claims;
using System.Threading.Tasks;
using E_commerce.Controllers.UserController;
using E_commerce.DTOs.User;
using E_commerce.Helpers;
using E_commerce.Services;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace E_commerce.Tests.User
{
    public class UserControllerTests
    {
        [Fact]
        public async Task GetProfile_ReturnsOk_WhenTokenIsValid()
        {
            var userId = Guid.NewGuid();
            var userService = new Mock<IUserService>();
            var response = new ServiceResponse<UserProfileResponse>
            {
                IsSuccess = true,
                Data = new UserProfileResponse
                {
                    Email = "user@example.com",
                    FullName = "Test User",
                    PhoneNumber = "0123456789",
                    Address = "1 Test St"
                }
            };

            userService.Setup(s => s.GetProfileAsync(userId)).ReturnsAsync(response);

            var controller = CreateController(userId, userService.Object);

            var result = await controller.GetProfile();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
            var payload = Assert.IsType<BaseResponse<UserProfileResponse>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Equal("user@example.com", payload.Data?.Email);
        }

        [Fact]
        public async Task UpdateProfile_ReturnsOk_WhenRequestIsValid()
        {
            var userId = Guid.NewGuid();
            var userService = new Mock<IUserService>();
            var request = new UserProfileUpdateRequest { PhoneNumber = "0999888777" };
            var response = new ServiceResponse<UserProfileResponse>
            {
                IsSuccess = true,
                Message = "Profile updated successfully.",
                Data = new UserProfileResponse
                {
                    Email = "user@example.com",
                    FullName = "Test User",
                    PhoneNumber = request.PhoneNumber ?? string.Empty,
                    Address = "1 Test St"
                }
            };

            userService.Setup(s => s.UpdateProfileAsync(userId, It.IsAny<UserProfileUpdateRequest>()))
                .ReturnsAsync(response);

            var controller = CreateController(userId, userService.Object);

            var result = await controller.UpdateProfile(request);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
            var payload = Assert.IsType<BaseResponse<UserProfileResponse>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Equal("0999888777", payload.Data?.PhoneNumber);
            userService.Verify(s => s.UpdateProfileAsync(userId, It.Is<UserProfileUpdateRequest>(r => r.PhoneNumber == "0999888777")), Times.Once);
        }

        private static UserController CreateController(Guid userId, IUserService userService)
        {
            var adminUserService = new Mock<IAdminUserService>();
            var controller = new UserController(adminUserService.Object, userService);
            var identity = new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Role, "Customer")
                },
                "TestAuth");

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            return controller;
        }
    }
}
