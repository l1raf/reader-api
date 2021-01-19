using System;
using System.ComponentModel.DataAnnotations;

namespace ReaderBackend.Dtos
{
    public class WebPageUpdateDto
    {
        [Required]
        public Uri Url { get; set; }

        [Required]
        public string Title { get; set; }
    }
}
