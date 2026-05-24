using E_commerce.DTOs.Products;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] ProductFilterRequest filter)
        {
            var result = await _productService.GetAll(filter);
            return Ok(BaseResponse<PagedResponse<ProductResponse>>.Ok(result));
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var result = await _productService.GetById(id);
                return Ok(BaseResponse<ProductResponse>.Ok(result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(BaseResponse<ProductResponse>.Fail(ex.Message, 404));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.CreateProduct(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _productService.UpdateProduct(id, request);
                return Ok(BaseResponse<ProductResponse>.Ok(result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(BaseResponse<ProductResponse>.Fail(ex.Message, 404));
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _productService.DeleteProduct(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(BaseResponse<string>.Fail(ex.Message, 404));
            }
        }
    }
}