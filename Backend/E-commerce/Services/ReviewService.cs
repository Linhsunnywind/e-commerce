using E_commerce.DTOs.Review;
using E_commerce.Helpers;
using E_commerce.Models;
using E_commerce.Repositories.Interfaces;
using E_commerce.Services.Interfaces;

namespace E_commerce.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<List<ReviewResponse>> GetProductReviews(
            Guid productId)
        {
            var product = await _reviewRepository
                .GetProductById(productId);

            if (product == null || product.Reviews == null)
            {
                return new List<ReviewResponse>();
            }

            return product.Reviews
                .Select(r => new ReviewResponse
                {
                    Id = r.Id,
                    UserName = r.User.FullName,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedDate = r.CreatedDate
                })
                .ToList();
        }
        public Task<bool> HasUserPurchasedProduct(Guid userId, Guid productId)
            => _reviewRepository.HasUserPurchasedProduct(userId, productId);

        public Task<bool> HasUserReviewed(Guid userId, Guid productId)
            => _reviewRepository.GetUserReviewForProduct(userId, productId)
                .ContinueWith(t => t.Result != null);

        public async Task<BaseResponse<string>> CreateReview(
            Guid productId,
            Guid userId,
            CreateReviewRequest request)
        {
            var hasPurchased = await _reviewRepository.HasUserPurchasedProduct(userId, productId);
            if (!hasPurchased)
                return BaseResponse<string>.Fail("You must purchase this product before reviewing", 403);

            // Check đã review chưa
            var existingReview = await _reviewRepository.GetUserReviewForProduct(userId, productId);
            if (existingReview != null)
                return BaseResponse<string>.Fail("You have already reviewed this product", 400);
            
            var product = await _reviewRepository
                .GetProductById(productId);

            if (product == null)
            {
                return BaseResponse<string>.Fail(
                    "Product not found",
                    404);
            }

            var review = new Review
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                UserId = userId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedDate = DateTime.UtcNow
            };

            await _reviewRepository.AddReview(review);

            await _reviewRepository.SaveChanges();

            product.AverageRating =
                await _reviewRepository
                    .CalculateAverageRating(productId);

            await _reviewRepository.SaveChanges();

            return BaseResponse<string>.Ok(
                "Review created successfully");
        }

        public async Task<BaseResponse<string>> UpdateReview(
            Guid productId,
            Guid reviewId,
            Guid userId,
            UpdateReviewRequest request)
        {
            var review = await _reviewRepository
                .GetReviewById(reviewId);

            if (review == null || review.ProductId != productId)
            {
                return BaseResponse<string>.Fail(
                    "Review not found",
                    404);
            }

            if (review.UserId != userId)
            {
                return BaseResponse<string>.Fail(
                    "Forbidden",
                    403);
            }

            review.Rating = request.Rating;
            review.Comment = request.Comment;

            var product = await _reviewRepository
                .GetProductById(productId);

            if (product != null)
            {
                product.AverageRating =
                    await _reviewRepository
                        .CalculateAverageRating(productId);
            }

            await _reviewRepository.SaveChanges();

            return BaseResponse<string>.Ok(
                "Review updated successfully");
        }

        public async Task<BaseResponse<string>> DeleteReview(
            Guid productId,
            Guid reviewId,
            Guid userId,
            bool isAdmin)
        {
            var review = await _reviewRepository
                .GetReviewById(reviewId);

            if (review == null || review.ProductId != productId)
            {
                return BaseResponse<string>.Fail(
                    "Review not found",
                    404);
            }

            if (review.UserId != userId && !isAdmin)
            {
                return BaseResponse<string>.Fail(
                    "Forbidden",
                    403);
            }

            _reviewRepository.DeleteReview(review);

            await _reviewRepository.SaveChanges();

            var product = await _reviewRepository
                .GetProductById(productId);

            if (product != null)
            {
                product.AverageRating =
                    await _reviewRepository
                        .CalculateAverageRating(productId);

                await _reviewRepository.SaveChanges();
            }

            return BaseResponse<string>.Ok(
                "Review deleted successfully");
        }
    }
}