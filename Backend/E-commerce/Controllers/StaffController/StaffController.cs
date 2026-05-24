using E_commerce.Data;
using E_commerce.DTOs.Staff;
using E_commerce.Helpers;
using E_commerce.Services;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Controllers.StaffController
{
    [Route("api/staffs")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;
        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _staffService.GetAllStaff();
            return Ok(BaseResponse<List<StaffResponse>>.Ok(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _staffService.GetStaffById(id);
            if (result == null)
            {
                return NotFound(BaseResponse<StaffResponse>.Fail("Staff not found", 404));
            }
            return Ok(BaseResponse<StaffResponse>.Ok(result));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateStaff staff)
        {
            var result = await _staffService.CreateStaff(staff);
            if (result != "Create successfully")
            {
                return BadRequest(BaseResponse<string>.Fail(result));
            }
            return StatusCode(201, BaseResponse<string>.Ok(null, "Create successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateStaff staff)
        {
            var result = await _staffService.UpdateStaff(id, staff);
            return Ok(BaseResponse<string>.Ok(null, result));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _staffService.DeleteStaff(id);
            return Ok(BaseResponse<string>.Ok(null, result));
        }
    }
}
