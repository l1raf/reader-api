using System;
using System.ComponentModel.DataAnnotations;

namespace ReaderBackend.DTOs
{
    public class WebPageUpdateDto
    {
        [Required]
        public Uri Uri { get; set; }

        public string Title { get; set; }

        public bool Favorite { get; set; }
    }
}
