﻿using System;

namespace ReaderBackend.Models.ArticleElements
{
    public class ImageElement : IArticleElement
    {
        public ElementType Type => ElementType.Image;

        public Uri Uri { get; set; }

        public ImageElement(Uri uri)
        {
            Uri = uri;
        }
    }
}
