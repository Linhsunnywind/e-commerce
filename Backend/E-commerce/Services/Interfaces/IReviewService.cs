using E_commerce.DTOs.Review;
using E_commerce.Helpers;

namespace E_commerce.Services.Interfaces
{
    public interface IReviewService
    {
        Task<List<ReviewResponse>> GetProductReviews(
            Guid productId);

        Task<BaseResponse<string>> CreateReview(
            Guid productId,
            Guid userId,
            CreateReviewRequest request);

        Task<BaseResponse<string>> UpdateReview(
            Guid productId,
            Guid reviewId,
            Guid userId,
            UpdateReviewRequest request);

        Task<BaseResponse<string>> DeleteReview(
            Guid productId,
            Guid reviewId,
            Guid userId,
            bool isAdmin);
        
        Task<bool> HasUserPurchasedProduct(Guid userId, Guid productId);
        Task<bool> HasUserReviewed(Guid userId, Guid productId);
    }
}