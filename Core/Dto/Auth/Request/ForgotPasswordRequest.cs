using System.ComponentModel.DataAnnotations;

namespace Core.Dto.Auth.Request
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
