using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReaderBackend.Models
{
    public class WebPage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Uri Uri { get; set; }

        [Required]
        public string Title { get; set; }
        
        public User User { get; set; }
        
        [ForeignKey("FK")]
        public Guid UserId { get; set; }
    }
}
