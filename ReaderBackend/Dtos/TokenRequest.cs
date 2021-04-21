using System.ComponentModel.DataAnnotations;

namespace ReaderBackend.DTOs
{
    public class TokenRequest
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}