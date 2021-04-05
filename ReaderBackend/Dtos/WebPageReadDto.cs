using System;

namespace ReaderBackend.DTOs
{
    public class WebPageReadDto
    {
        public Guid Id { get; set; }

        public Uri Url { get; set; }

        public string Title { get; set; }
    }
}
