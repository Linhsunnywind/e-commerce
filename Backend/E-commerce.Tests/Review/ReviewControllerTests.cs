using E_commerce.Controllers.ReviewController;
using E_commerce.DTOs.Review;
using E_commerce.Helpers;
using E_commerce.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace E_commerce.Tests.Review
{
    public class ReviewControllerTests
    {
        private readonly Mock<IReviewService> _service = new();
        private readonly Guid _userId = Guid.NewGuid();
        private readonly ReviewController _controller;

        public ReviewControllerTests()
        {
            _controller = new ReviewController(_service.Object);
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _userId.ToString()),
                new Claim(ClaimTypes.Role, "Customer")
            }, "TestAuth");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        [Fact]
        public async Task GetReviews_ReturnsOk_WithReviewList()
        {
            var productId = Guid.NewGuid();
            _service.Setup(s => s.GetProductReviews(productId)).ReturnsAsync(new List<ReviewResponse>
            {
                new ReviewResponse { Id = Guid.NewGuid(), UserName = "Nguyen Van A", Rating = 5, Comment = "Sản phẩm tốt", CreatedDate = DateTime.UtcNow }
            });

            var result = await _controller.GetReviews(productId);

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<BaseResponse<List<ReviewResponse>>>(ok.Value);
            Assert.True(payload.Success);
            Assert.Single(payload.Data!);
        }

        [Fact]
        public async Task CanReview_ReturnsOk_WhenUserCanReview()
        {
            var productId = Guid.NewGuid();
            _service.Setup(s => s.HasUserPurchasedProduct(_userId, productId)).ReturnsAsync(true);
            _service.Setup(s => s.HasUserReviewed(_userId, productId)).ReturnsAsync(false);

            var result = await _controller.CanReview(productId);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task CreateReview_ReturnsOk_WhenReviewCreated()
        {
            var productId = Guid.NewGuid();
            _service.Setup(s => s.CreateReview(productId, _userId, It.IsAny<CreateReviewRequest>()))
                .ReturnsAsync(BaseResponse<string>.Ok("Review created successfully"));

            var result = await _controller.CreateReview(productId, new CreateReviewRequest { Rating = 5, Comment = "Rất hài lòng" });

            var ok = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task CreateReview_ReturnsForbid_WhenUserHasNotPurchased()
        {
            var productId = Guid.NewGuid();
            _service.Setup(s => s.CreateReview(productId, _userId, It.IsAny<CreateReviewRequest>()))
                .ReturnsAsync(BaseResponse<string>.Fail("You must purchase this product before reviewing", 403));

            var result = await _controller.CreateReview(productId, new CreateReviewRequest { Rating = 4 });

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task UpdateReview_ReturnsOk_WhenUpdated()
        {
            var productId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            _service.Setup(s => s.UpdateReview(productId, reviewId, _userId, It.IsAny<UpdateReviewRequest>()))
                .ReturnsAsync(BaseResponse<string>.Ok("Review updated successfully"));

            var result = await _controller.UpdateReview(productId, reviewId, new UpdateReviewRequest { Rating = 4, Comment = "Tạm ổn" });

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdateReview_ReturnsNotFound_WhenReviewNotFound()
        {
            var productId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            _service.Setup(s => s.UpdateReview(productId, reviewId, _userId, It.IsAny<UpdateReviewRequest>()))
                .ReturnsAsync(BaseResponse<string>.Fail("Review not found", 404));

            var result = await _controller.UpdateReview(productId, reviewId, new UpdateReviewRequest { Rating = 3 });

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteReview_ReturnsOk_WhenDeleted()
        {
            var productId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            _service.Setup(s => s.DeleteReview(productId, reviewId, _userId, false))
                .ReturnsAsync(BaseResponse<string>.Ok("Review deleted successfully"));

            var result = await _controller.DeleteReview(productId, reviewId);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteReview_ReturnsNotFound_WhenReviewNotFound()
        {
            var productId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            _service.Setup(s => s.DeleteReview(productId, reviewId, _userId, false))
                .ReturnsAsync(BaseResponse<string>.Fail("Review not found", 404));

            var result = await _controller.DeleteReview(productId, reviewId);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
