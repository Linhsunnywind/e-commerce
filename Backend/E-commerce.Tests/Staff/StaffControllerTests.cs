using E_commerce.Controllers.StaffController;
using E_commerce.DTOs.Staff;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace E_commerce.Tests.Staff
{
    public class StaffControllerTests
    {
        private readonly Mock<IStaffService> _service = new();
        private readonly StaffController _controller;

        public StaffControllerTests()
        {
            _controller = new StaffController(_service.Object);
        }

        private StaffResponse FakeStaff() => new StaffResponse
        {
            Id = Guid.NewGuid(),
            Name = "staff01",
            Email = "staff01@techshop.vn",
            FullName = "Nguyen Thi B",
            PhoneNumber = "0912345678",
            RoleName = "Staff"
        };

        [Fact]
        public async Task GetAll_ReturnsOk_WithStaffList()
        {
            _service.Setup(s => s.GetAllStaff()).ReturnsAsync(new List<StaffResponse> { FakeStaff() });

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<List<StaffResponse>>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Single(payload.Data!);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenStaffFound()
        {
            var staff = FakeStaff();
            _service.Setup(s => s.GetStaffById(staff.Id)).ReturnsAsync(staff);

            var result = await _controller.GetById(staff.Id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<StaffResponse>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenStaffNotFound()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.GetStaffById(id)).ReturnsAsync((StaffResponse?)null);

            var result = await _controller.GetById(id);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Create_Returns201_WhenStaffCreated()
        {
            _service.Setup(s => s.CreateStaff(It.IsAny<CreateStaff>())).ReturnsAsync("Create successfully");

            var result = await _controller.Create(new CreateStaff
            {
                Name = "staff02",
                Password = "Pass@123",
                Email = "staff02@techshop.vn",
                PhoneNumber = "0987654321"
            });

            var created = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, created.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenEmailAlreadyExists()
        {
            _service.Setup(s => s.CreateStaff(It.IsAny<CreateStaff>())).ReturnsAsync("Email already exists");

            var result = await _controller.Create(new CreateStaff
            {
                Name = "staff02",
                Password = "Pass@123",
                Email = "dup@techshop.vn",
                PhoneNumber = "0987654321"
            });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenUpdated()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.UpdateStaff(id, It.IsAny<UpdateStaff>())).ReturnsAsync("Updated successfully");

            var result = await _controller.Update(id, new UpdateStaff { Name = "staff02", PhoneNumber = "0911111111" });

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<string>>(ok.Value);
            Assert.True(payload.Success);
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenDeleted()
        {
            var id = Guid.NewGuid();
            _service.Setup(s => s.DeleteStaff(id)).ReturnsAsync("Deleted successfully");

            var result = await _controller.Delete(id);

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<string>>(ok.Value);
            Assert.True(payload.Success);
        }
    }
}
