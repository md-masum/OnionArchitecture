using System.Threading.Tasks;
using Core.Common;
using Core.Dto.Auth.Request;
using Core.Dto.Auth.Response;

namespace Core.Interfaces.Common
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
        Task<ApiResponse<string>> RegisterAsync(SignUpRequest request);
        Task ForgotPassword(ForgotPasswordRequest model);
        Task<ApiResponse<string>> ResetPassword(ResetPasswordRequest model);
        Task<ApiResponse<string>> ChangePassword(ChangePwdRequest model);
        Task<bool> CheckUserByEmail(string email);
        Task<bool> CheckUserByPhone(string phoneNumber);
        Task<bool> ActivateUser(string userId);
        Task<bool> DeactivateUser(string userId);
    }
}
