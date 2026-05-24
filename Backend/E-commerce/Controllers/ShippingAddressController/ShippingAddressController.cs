using E_commerce.DTOs.ShippingAddress;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_commerce.Controllers
{
    [ApiController]
    [Route("api/shipping-addresses")]
    [Authorize(Roles = "Customer")]
    public class ShippingAddressController : ControllerBase
    {
        private readonly IShippingAddressService _service;

        public ShippingAddressController(IShippingAddressService service)
        {
            _service = service;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetByUserAsync(GetUserId());
            return Ok(BaseResponse<List<ShippingAddressDto>>.Ok(result));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShippingAddressRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.CreateAsync(GetUserId(), request);
            return Ok(BaseResponse<ShippingAddressDto>.Ok(result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateShippingAddressRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _service.UpdateAsync(GetUserId(), id, request);
                return Ok(BaseResponse<ShippingAddressDto>.Ok(result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(BaseResponse<string>.Fail(ex.Message, 404));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteAsync(GetUserId(), id);
                return Ok(BaseResponse<string>.Ok(string.Empty, "Deleted."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(BaseResponse<string>.Fail(ex.Message, 404));
            }
        }

        [HttpPatch("{id}/default")]
        public async Task<IActionResult> SetDefault(Guid id)
        {
            try
            {
                await _service.SetDefaultAsync(GetUserId(), id);
                return Ok(BaseResponse<string>.Ok(string.Empty, "Default updated."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(BaseResponse<string>.Fail(ex.Message, 404));
            }
        }
    }
}
