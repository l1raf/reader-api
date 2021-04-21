using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReaderBackend.Models
{
    public class RefreshToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        public Guid UserId { get; set; }
        
        public string Token { get; set; }
        
        public string JwtId { get; set; }
        
        public bool IsUsed { get; set; }
        
        public bool IsRevoked { get; set; }
        
        public DateTime AddedDate { get; set; }
        
        public DateTime ExpiryDate { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}