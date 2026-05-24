using BCrypt.Net;
using E_commerce.DTOs.User;
using E_commerce.Repositories.Interfaces;
using E_commerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ServiceResponse<UserProfileResponse>> GetProfileAsync(Guid userId)
        {
            var response = new ServiceResponse<UserProfileResponse>();
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                response.IsSuccess = false;
                response.Message = "Account does not exist.";
                return response;
            }

            response.Data = new UserProfileResponse
            {
                Email = user.Email,
                FullName = user.FullName ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address ?? string.Empty
            };
            response.IsSuccess = true;
            return response;
        }

        public async Task<ServiceResponse<UserProfileResponse>> UpdateProfileAsync(Guid userId, UserProfileUpdateRequest dto)
        {
            var response = new ServiceResponse<UserProfileResponse>();
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                response.IsSuccess = false;
                response.Message = "Account doesn't exist.";
                return response;
            }

            if (string.IsNullOrWhiteSpace(dto.FullName)
                && string.IsNullOrWhiteSpace(dto.PhoneNumber)
                && dto.Address == null)
            {
                response.IsSuccess = false;
                response.Message = "No fields provided to update.";
                return response;
            }

            if (!string.IsNullOrEmpty(dto.PhoneNumber)
                && !await _userRepository.IsPhoneNumberUniqueAsync(dto.PhoneNumber, user.Id))
            {
                response.IsSuccess = false;
                response.Message = "Phone number is already in use.";
                return response;
            }

            if (!string.IsNullOrWhiteSpace(dto.FullName))
            {
                user.FullName = dto.FullName.Trim();
                user.Name = dto.FullName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                user.PhoneNumber = dto.PhoneNumber.Trim();
            }

            if (dto.Address != null)
            {
                user.Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim();
            }

            var saveResult = await _userRepository.SaveChangesAsync();
            if (!saveResult)
            {
                response.IsSuccess = false;
                response.Message = "Failed to update profile.";
                return response;
            }

            response.IsSuccess = true;
            response.Message = "Profile updated successfully.";
            response.Data = new UserProfileResponse
            {
                Email = user.Email,
                FullName = user.FullName ?? user.Name,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address ?? string.Empty
            };
            return response;
        }

        public async Task<ServiceResponse<string>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            var response = new ServiceResponse<string>();
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                response.IsSuccess = false;
                response.Message = "Account does not exist.";
                return response;
            }

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password))
            {
                response.IsSuccess = false;
                response.Message = "Mật khẩu hiện tại không đúng.";
                return response;
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _userRepository.SaveChangesAsync();

            response.IsSuccess = true;
            response.Message = "Đổi mật khẩu thành công.";
            return response;
        }

        public async Task<ServiceResponse<string>> DeleteAccountAsync(Guid userId)
        {
            var response = new ServiceResponse<string>();
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                response.IsSuccess = false;
                response.Message = "Account does not exist.";
                return response;
            }

            user.IsDeleted = true;

            var saveResult = await _userRepository.SaveChangesAsync();
            if (!saveResult)
            {
                response.IsSuccess = false;
                response.Message = "Failed to delete account.";
                return response;
            }

            response.IsSuccess = true;
            response.Message = "Account deleted successfully.";
            return response;
        }
    }
}
