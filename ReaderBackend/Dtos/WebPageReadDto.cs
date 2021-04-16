using System;

namespace ReaderBackend.DTOs
{
    public class WebPageReadDto
    {
        public Guid Id { get; set; }

        public Uri Uri { get; set; }

        public string Title { get; set; }
    }
}
