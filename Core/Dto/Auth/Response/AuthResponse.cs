using System.Collections.Generic;

namespace Core.Dto.Auth.Response
{
    public class AuthResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
    }
}
