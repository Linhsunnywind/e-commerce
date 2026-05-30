using E_commerce.DTOs.Admin;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace E_commerce.Controllers
{
    [Route("api/admins")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // GET /api/admin/customers
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _adminService.GetUsersByRoleAsync("Customer");
            return Ok(BaseResponse<IEnumerable<UserListResponseDto>>.Ok(customers));
        }

        // GET /api/admin/staff
        [HttpGet("staff")]
        public async Task<IActionResult> GetStaff()
        {
            var staff = await _adminService.GetUsersByRoleAsync("Staff");
            return Ok(BaseResponse<IEnumerable<UserListResponseDto>>.Ok(staff));
        }
    }
}