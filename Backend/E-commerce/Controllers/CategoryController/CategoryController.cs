using E_commerce.DTOs.Category;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Controllers.CategoryController
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _categoryService.GetCategories();

            return Ok(BaseResponse<List<CategoryResponse>>.Ok(result));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory(CategoryRequest request)
        {
            try
            {
                var result = await _categoryService.CreateCategory(request);

                return StatusCode(
                    201,
                    BaseResponse<CategoryResponse>.Ok(result, "Category created successfully")
                );
            }
            catch (Exception ex)
            {
                return BadRequest(BaseResponse<string>.Fail(ex.Message, 400));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(Guid id, CategoryRequest request)
        {
            var result = await _categoryService.UpdateCategory(id, request);

            return Ok(BaseResponse<CategoryResponse>.Ok(result));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            await _categoryService.DeleteCategory(id);

            return Ok(BaseResponse<string>.Ok(null, "Deleted successfully"));
        }
    }
}
