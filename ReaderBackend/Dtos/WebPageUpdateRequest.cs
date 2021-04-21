using System;
using System.ComponentModel.DataAnnotations;

namespace ReaderBackend.DTOs
{
    public class WebPageUpdateDto
    {
        [Required]
        public Uri Uri { get; set; }

        [Required]
        public string Title { get; set; }
    }
}
