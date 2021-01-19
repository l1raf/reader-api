using System;
using System.ComponentModel.DataAnnotations;

namespace ReaderBackend.Models
{
    public class WebPage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Uri Url { get; set; }

        [Required]
        public string Title { get; set; }
    }
}
