using E_commerce.DTOs.Brand;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Controllers.BrandController
{
    [ApiController]
    [Route("api/brands")]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBrands()
        {
            var result = await _brandService.GetBrands();

            return Ok(BaseResponse<List<BrandResponse>>.Ok(result));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBrand(BrandRequest request)
        {
            try
            {
                var result = await _brandService.CreateBrand(request);

                return StatusCode(
                    201,
                    BaseResponse<BrandResponse>.Ok(result, "Brand created successfully")
                );
            }
            catch (Exception ex)
            {
                return BadRequest(BaseResponse<string>.Fail(ex.Message, 400));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBrand(Guid id, BrandRequest request)
        {
            var result = await _brandService.UpdateBrand(id, request);

            return Ok(BaseResponse<BrandResponse>.Ok(result));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBrand(Guid id)
        {
            await _brandService.DeleteBrand(id);

            return Ok(BaseResponse<string>.Ok(null, "Deleted successfully"));
        }
    }
}
