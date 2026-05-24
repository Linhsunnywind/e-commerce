using System.Security.Claims;
using E_commerce.DTOs.Review;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Controllers.ReviewController
{
    [ApiController]
    [Route("api/products")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("{id}/reviews")]
        public async Task<IActionResult> GetReviews(Guid id)
        {
            var result = await _reviewService.GetProductReviews(id);

            return Ok(
                BaseResponse<List<ReviewResponse>>
                    .Ok(result));
        }

        [HttpPost("{id}/reviews")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateReview(
            Guid id,
            CreateReviewRequest request)
        {
            var userId = Guid.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);

            var result = await _reviewService
                .CreateReview(id, userId, request);

            if (!result.Success)
            {
                return result.StatusCode switch
                {
                    404 => NotFound(result),
                    403 => StatusCode(403, result),
                    _ => BadRequest(result)
                };
            }

            return Ok(result);
        }

        [HttpPut("{id}/reviews/{reviewId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateReview(
            Guid id,
            Guid reviewId,
            UpdateReviewRequest request)
        {
            var userId = Guid.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);

            var result = await _reviewService
                .UpdateReview(
                    id,
                    reviewId,
                    userId,
                    request);

            if (!result.Success)
            {
                return result.StatusCode switch
                {
                    404 => NotFound(result),
                    403 => StatusCode(403, result),
                    _ => BadRequest(result)
                };
            }

            return Ok(result);
        }

        [HttpDelete("{id}/reviews/{reviewId}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(
            Guid id,
            Guid reviewId)
        {
            var userId = Guid.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);

            var isAdmin = User.IsInRole("Admin");

            var result = await _reviewService
                .DeleteReview(
                    id,
                    reviewId,
                    userId,
                    isAdmin);

            if (!result.Success)
            {
                return result.StatusCode switch
                {
                    404 => NotFound(result),
                    403 => StatusCode(403, result),
                    _ => BadRequest(result)
                };
            }

            return Ok(result);
        }
    }
}