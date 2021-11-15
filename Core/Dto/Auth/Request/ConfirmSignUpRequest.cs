using System.ComponentModel.DataAnnotations;

namespace Core.Dto.Auth.Request
{
    public class ConfirmSignUpRequest
    {
        [Required]
        public string ConfirmationCode { get; set; }
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}
