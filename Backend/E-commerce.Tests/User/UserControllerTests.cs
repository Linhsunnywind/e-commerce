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

        [Fact]
        public async Task GetProfile_ReturnsBadRequest_WhenServiceFails()
        {
            var userId = Guid.NewGuid();
            var userService = new Mock<IUserService>();
            userService.Setup(s => s.GetProfileAsync(userId))
                .ReturnsAsync(new ServiceResponse<UserProfileResponse> { IsSuccess = false, Message = "User not found." });

            var controller = CreateController(userId, userService.Object);

            var result = await controller.GetProfile();

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, bad.StatusCode);
            var payload = Assert.IsType<BaseResponse<UserProfileResponse>>(bad.Value);
            Assert.False(payload.Success);
        }

        [Fact]
        public async Task UpdateProfile_ReturnsBadRequest_WhenServiceFails()
        {
            var userId = Guid.NewGuid();
            var userService = new Mock<IUserService>();
            userService.Setup(s => s.UpdateProfileAsync(userId, It.IsAny<UserProfileUpdateRequest>()))
                .ReturnsAsync(new ServiceResponse<UserProfileResponse> { IsSuccess = false, Message = "Update failed." });

            var controller = CreateController(userId, userService.Object);

            var result = await controller.UpdateProfile(new UserProfileUpdateRequest { PhoneNumber = "0999888777" });

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, bad.StatusCode);
            var payload = Assert.IsType<BaseResponse<UserProfileResponse>>(bad.Value);
            Assert.False(payload.Success);
        }

        [Fact]
        public async Task ChangePassword_ReturnsOk_WhenPasswordChanged()
        {
            var userId = Guid.NewGuid();
            var userService = new Mock<IUserService>();
            var request = new ChangePasswordRequest { CurrentPassword = "Old@123", NewPassword = "New@123" };
            userService.Setup(s => s.ChangePasswordAsync(userId, It.IsAny<ChangePasswordRequest>()))
                .ReturnsAsync(new ServiceResponse<string> { IsSuccess = true, Message = "Password changed.", Data = "ok" });

            var controller = CreateController(userId, userService.Object);

            var result = await controller.ChangePassword(request);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
            var payload = Assert.IsType<BaseResponse<string>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task ChangePassword_ReturnsBadRequest_WhenOldPasswordWrong()
        {
            var userId = Guid.NewGuid();
            var userService = new Mock<IUserService>();
            userService.Setup(s => s.ChangePasswordAsync(userId, It.IsAny<ChangePasswordRequest>()))
                .ReturnsAsync(new ServiceResponse<string> { IsSuccess = false, Message = "Old password is incorrect." });

            var controller = CreateController(userId, userService.Object);

            var result = await controller.ChangePassword(new ChangePasswordRequest { CurrentPassword = "Wrong@1", NewPassword = "New@123" });

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, bad.StatusCode);
            var payload = Assert.IsType<BaseResponse<string>>(bad.Value);
            Assert.False(payload.Success);
        }

        [Fact]
        public async Task DeleteProfile_ReturnsOk_WhenAccountDeleted()
        {
            var userId = Guid.NewGuid();
            var userService = new Mock<IUserService>();
            userService.Setup(s => s.DeleteAccountAsync(userId))
                .ReturnsAsync(new ServiceResponse<string> { IsSuccess = true, Message = "Account deleted.", Data = "ok" });

            var controller = CreateController(userId, userService.Object);

            var result = await controller.DeleteProfile();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
            var payload = Assert.IsType<BaseResponse<string>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task DeleteProfile_ReturnsBadRequest_WhenServiceFails()
        {
            var userId = Guid.NewGuid();
            var userService = new Mock<IUserService>();
            userService.Setup(s => s.DeleteAccountAsync(userId))
                .ReturnsAsync(new ServiceResponse<string> { IsSuccess = false, Message = "Delete failed." });

            var controller = CreateController(userId, userService.Object);

            var result = await controller.DeleteProfile();

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, bad.StatusCode);
            var payload = Assert.IsType<BaseResponse<string>>(bad.Value);
            Assert.False(payload.Success);
        }

        [Fact]
        public async Task CreateStaffOrAdmin_ReturnsOk_WhenRequestIsValid()
        {
            var userId = Guid.NewGuid();
            var userService = new Mock<IUserService>();
            var adminUserService = new Mock<IAdminUserService>();
            var request = new AdminCreateUserRequest
            {
                Name = "Staff01",
                Email = "staff@example.com",
                PhoneNumber = "0123456789",
                Password = "Staff@123",
                RoleName = "Staff"
            };
            adminUserService.Setup(s => s.CreateStaffOrAdminAsync(It.IsAny<AdminCreateUserRequest>()))
                .ReturnsAsync(new ServiceResponse<AdminCreateUserResponse>
                {
                    IsSuccess = true,
                    Message = "Created successfully.",
                    Data = new AdminCreateUserResponse { Email = request.Email, Name = request.Name, RoleName = request.RoleName }
                });

            var controller = CreateController(userId, userService.Object, adminUserService.Object);

            var result = await controller.CreateStaffOrAdmin(request);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
            var payload = Assert.IsType<BaseResponse<AdminCreateUserResponse>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Equal("staff@example.com", payload.Data?.Email);
        }

        [Fact]
        public async Task CreateStaffOrAdmin_ReturnsBadRequest_WhenEmailAlreadyExists()
        {
            var userId = Guid.NewGuid();
            var userService = new Mock<IUserService>();
            var adminUserService = new Mock<IAdminUserService>();
            adminUserService.Setup(s => s.CreateStaffOrAdminAsync(It.IsAny<AdminCreateUserRequest>()))
                .ReturnsAsync(new ServiceResponse<AdminCreateUserResponse> { IsSuccess = false, Message = "Email already exists." });

            var controller = CreateController(userId, userService.Object, adminUserService.Object);

            var result = await controller.CreateStaffOrAdmin(new AdminCreateUserRequest
            {
                Name = "Staff01", Email = "dup@example.com", PhoneNumber = "0123456789",
                Password = "Staff@123", RoleName = "Staff"
            });

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, bad.StatusCode);
            var payload = Assert.IsType<BaseResponse<AdminCreateUserResponse>>(bad.Value);
            Assert.False(payload.Success);
        }

        private static UserController CreateController(Guid userId, IUserService userService, IAdminUserService? adminUserService = null)
        {
            var adminMock = adminUserService ?? new Mock<IAdminUserService>().Object;
            var controller = new UserController(adminMock, userService);
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
