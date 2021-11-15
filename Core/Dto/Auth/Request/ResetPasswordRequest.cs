using System.ComponentModel.DataAnnotations;

namespace Core.Dto.Auth.Request
{
    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Required]
        public string ResetPasswordToken { get; set; }
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
