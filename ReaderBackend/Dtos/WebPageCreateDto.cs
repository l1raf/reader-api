using System;
using System.ComponentModel.DataAnnotations;

namespace ReaderBackend.Dtos
{
    public class WebPageCreateDto
    {
        [Required]
        public Uri Url { get; set; }

        [Required]
        public string Title { get; set; }
    }
}
