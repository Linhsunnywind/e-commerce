using E_commerce.Controllers;
using E_commerce.DTOs.Admin;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace E_commerce.Tests.Admin
{
    public class AdminControllerTests
    {
        private readonly Mock<IAdminService> _service = new();
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _controller = new AdminController(_service.Object);
        }

        [Fact]
        public async Task GetCustomers_ReturnsOk_WithCustomerList()
        {
            _service.Setup(s => s.GetUsersByRoleAsync("Customer"))
                .ReturnsAsync(new List<UserListResponseDto>
                {
                    new UserListResponseDto { Id = Guid.NewGuid(), FullName = "Nguyen Van A", Email = "vana@example.com", RoleName = "Customer" },
                    new UserListResponseDto { Id = Guid.NewGuid(), FullName = "Tran Thi B", Email = "thib@example.com", RoleName = "Customer" }
                });

            var result = await _controller.GetCustomers();

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<IEnumerable<UserListResponseDto>>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Equal(2, payload.Data!.Count());
        }

        [Fact]
        public async Task GetStaff_ReturnsOk_WithStaffList()
        {
            _service.Setup(s => s.GetUsersByRoleAsync("Staff"))
                .ReturnsAsync(new List<UserListResponseDto>
                {
                    new UserListResponseDto { Id = Guid.NewGuid(), FullName = "Le Van C", Email = "levanc@techshop.vn", RoleName = "Staff" }
                });

            var result = await _controller.GetStaff();

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<IEnumerable<UserListResponseDto>>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Single(payload.Data!);
        }
    }
}
