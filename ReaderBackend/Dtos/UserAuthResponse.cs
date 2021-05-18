using System;

namespace ReaderBackend.DTOs
{
    public class UserAuthResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
