using TaskManagementSystem.DTOs.Auth.Requests;
using TaskManagementSystem.DTOs.Auth.Responses;

namespace TaskManagementSystem.Services.IServices
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
