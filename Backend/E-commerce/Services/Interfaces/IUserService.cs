using E_commerce.DTOs.User;

namespace E_commerce.Services.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResponse<UserProfileResponse>> GetProfileAsync(Guid userId);
        Task<ServiceResponse<UserProfileResponse>> UpdateProfileAsync(Guid userId, UserProfileUpdateRequest profile);
        Task<ServiceResponse<string>> DeleteAccountAsync(Guid userId);
        Task<ServiceResponse<string>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    }
}
