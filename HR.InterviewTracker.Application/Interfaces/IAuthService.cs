using HR.InterviewTracker.Application.DTOs;

namespace HR.InterviewTracker.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);

    // User management (Admin only)
    Task<UserResponseDto> CreateUserAsync(CreateUserDto request);
    Task<List<UserResponseDto>> GetAllUsersAsync();
    Task<UserResponseDto?> GetUserByIdAsync(string userId);
    Task<bool> DeactivateUserAsync(string userId);
}