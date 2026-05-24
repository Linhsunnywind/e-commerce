using E_commerce.DTOs.Image;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Controllers
{
    [ApiController]
    [Route("api")]
    public class ProductImageController : ControllerBase
    {
        private readonly IProductImageService _productImageService;
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        public ProductImageController(
            IProductImageService productImageService,
            CloudinaryDotNet.Cloudinary cloudinary)
        {
            _productImageService = productImageService;
            _cloudinary = cloudinary;
        }
        // POST api/products/{id}/images
        [HttpPost("products/{id}/images")]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> AddImage(Guid id, AddImageRequest request)
        {
            try
            {
                await _productImageService.AddImageAsync(id, request);
                return Ok(BaseResponse<string>.Ok(string.Empty, "Add URL image successfully."));
            }catch (KeyNotFoundException ex)
            {
                return NotFound(BaseResponse<string>.Fail(ex.Message, 404));
            }
        }

        // DELETE api/images/{id}
        [HttpDelete("images/{id}")]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> DeleteImage(Guid id)
        {
            try
            {
               await _productImageService.DeleteImageAsync(id);
                return Ok(BaseResponse<string>.Ok(string.Empty, "Delete URL image successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(BaseResponse<string>.Fail(ex.Message, 404));
            }
        }
        [HttpPost("images/upload")]
        [Authorize(Roles = "Admin,Staff")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest request)
        {
            var file = request.File;
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            using var stream = file.OpenReadStream();
            var uploadParams = new CloudinaryDotNet.Actions.ImageUploadParams
            {
                File = new CloudinaryDotNet.FileDescription(file.FileName, stream),
                Folder = "ecommerce/products"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                return BadRequest(result.Error.Message);

            return Ok(BaseResponse<string>.Ok(result.SecureUrl.ToString()));
        }
    }
}
