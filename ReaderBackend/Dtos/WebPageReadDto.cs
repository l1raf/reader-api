using System;

namespace ReaderBackend.Dtos
{
    public class WebPageReadDto
    {
        public int Id { get; set; }

        public Uri Url { get; set; }

        public string Title { get; set; }
    }
}
