using E_commerce.DTOs.SupportRequest;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace E_commerce.Controllers
{
    [Route("api/supports")]
    [ApiController]
    public class SupportController : ControllerBase
    {
        private readonly ISupportService _supportService;

        public SupportController(ISupportService supportService)
        {
            _supportService = supportService;
        }

        // POST /api/support — Customer
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateTicket([FromBody] CreateSupportRequestDto dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized();

            var result = await _supportService.CreateTicketAsync(userId, dto);
           return Ok(BaseResponse<SupportRequestResponseDto>.Ok(result));
        }

        // GET /api/support — Customer
        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyTickets()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized();

            var results = await _supportService.GetTicketsByUserIdAsync(userId);
            return Ok(BaseResponse<IEnumerable<SupportRequestResponseDto>>.Ok(results));
        }

        // PATCH /api/supports/{id}/status — Staff, Admin
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateSupportStatusDto dto)
        {
            try
            {
                var result = await _supportService.UpdateTicketStatusAsync(id, dto.Status);
                return Ok(BaseResponse<SupportRequestResponseDto>.Ok(result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(BaseResponse<string>.Fail(ex.Message, 404));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(BaseResponse<string>.Fail(ex.Message, 400));
            }
        }

        // GET /api/support/all — Staff, Admin
        [HttpGet("all")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAllTickets([FromQuery] string? status)
        {
            var results = await _supportService.GetAllTicketsAsync(status);
            return Ok(BaseResponse<IEnumerable<SupportRequestResponseDto>>.Ok(results));
        }
    }
}