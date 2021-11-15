using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Core.Common;
using Core.Dto.Auth.Request;
using Core.Dto.Auth.Response;
using Core.Entity;
using Core.Enums;
using Core.Exceptions;
using Core.Helpers;
using Core.Interfaces.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;

namespace Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMailService _mailService;
        private readonly JwtSettings _jwtSettings;

        public AuthService(UserManager<ApplicationUser> userManager,
            JwtSettings jwtSettings,
            SignInManager<ApplicationUser> signInManager,
            IMailService mailService)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _signInManager = signInManager;
            _mailService = mailService;
        }

        public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user is null)
            {
                throw new AuthException($"No Accounts Registered with {request.UserName}.");
            }

            if (!user.IsActive)
            {
                throw new CustomException($"User is not active");
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                throw new CustomException($"Invalid Credentials for '{request.UserName}'.");
            }

            var response = await GetAuthResponse(user);
            return new ApiResponse<AuthResponse>(response, $"Authenticated {user.UserName}");
        }

        private async Task<AuthResponse> GetAuthResponse(ApplicationUser user)
        {
            JwtSecurityToken jwtSecurityToken = await GenerateJwtToken(user);

            AuthResponse response = new AuthResponse();
            response.Id = user.Id;
            response.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.EmailAddress = user.Email;
            response.UserName = user.UserName;
            var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            response.Roles = rolesList.ToList();
            return response;
        }

        public async Task<ApiResponse<string>> RegisterAsync(SignUpRequest request)
        {
            var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
            if (userWithSameUserName != null)
            {
                throw new AuthException($"Username '{request.UserName}' is already taken.");
            }
            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.EmailAddress,
                UserName = request.UserName,
                EmailConfirmed = true,
                IsActive = true
            };
            var userWithSameEmail = await _userManager.FindByEmailAsync(request.EmailAddress);
            if (userWithSameEmail == null)
            {
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Roles.User.ToString());
                    return new ApiResponse<string>(user.Id, message: $"User Registered.");
                }

                throw new CustomException($"{result.Errors}");
            }

            throw new CustomException($"Email {request.EmailAddress } is already registered.");
        }

        public async Task<bool> ActivateUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new AuthException($"No user found.");
            }

            user.IsActive = true;

            var data = await _userManager.UpdateAsync(user);
            if (data.Succeeded)
            {
                return true;
            }

            throw new AuthException($"Can't update user");
        }

        public async Task<bool> DeactivateUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new AuthException($"No user found.");
            }

            user.IsActive = false;

            var data = await _userManager.UpdateAsync(user);
            if (data.Succeeded)
            {
                return true;
            }

            throw new AuthException($"Can't update user");
        }

        public async Task ForgotPassword(ForgotPasswordRequest model)
        {
            var account = await _userManager.FindByEmailAsync(model.Email);
            if (account is null) return;
            var passwordResetUri = await SendResetPasswordEmail(account);

            await _mailService.SendEmail(model.Email, account.UserName, "Reset Password", $"You reset token is - {passwordResetUri}");
        }

        public async Task<ApiResponse<string>> ResetPassword(ResetPasswordRequest model)
        {
            try
            {
                var account = await _userManager.FindByEmailAsync(model.EmailAddress);
                if (account is null) throw new CustomException($"No Accounts Registered with {model.EmailAddress}.");

                var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.ResetPasswordToken));

                var result = await _userManager.ResetPasswordAsync(account, token, model.NewPassword);
                if (result.Succeeded)
                {
                    return new ApiResponse<string>(model.EmailAddress, message: $"Password Reset Successful.");
                }
                throw new CustomException($"Error occured while reseting the password.");
            }
            catch (Exception e)
            {
                throw new CustomException($"Error occured while reseting the password. {e.Message}");
            }
        }

        public async Task<ApiResponse<string>> ChangePassword(ChangePwdRequest model)
        {
            try
            {
                var account = await _userManager.FindByEmailAsync(model.EmailAddress);
                if (account is null) throw new CustomException($"No Accounts Registered with {model.EmailAddress}.");

                var checkOldPassword = await _signInManager.PasswordSignInAsync(account.UserName, model.CurrentPassword, false, lockoutOnFailure: false);
                if (!checkOldPassword.Succeeded)
                {
                    throw new CustomException($"Invalid Credentials for '{account.Email}'.");
                }

                var result = await _userManager.ChangePasswordAsync(account, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return new ApiResponse<string>(account.Email, "Password changed");
                }
                throw new CustomException($"Error occured while change the password.");
            }
            catch (Exception e)
            {
                throw new CustomException($"Error occured while change the password. {e.Message}");
            }
        }

        public async Task<bool> CheckUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> CheckUserByPhone(string phoneNumber)
        {
            var user = await _userManager.FindByNameAsync(phoneNumber);
            if (user != null)
            {
                return true;
            }

            return false;
        }

        #region Helper Method

        private async Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("roles", role));
            }

            var claims = new[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Sid, Guid.NewGuid().ToString()),
                }
                .Union(userClaims)
                .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        private async Task<string> SendResetPasswordEmail(ApplicationUser user)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            return code;
        }

        #endregion
    }
}
