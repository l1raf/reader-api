using HtmlAgilityPack;
using ReaderBackend.Scraper.Models;
using ReaderBackend.Scraper.Models.ArticleElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReaderBackend.Scraper
{
    public class ArticleScraper : IArticleScraper
    {
        private readonly HttpClient _httpClient;

        public ArticleScraper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Article> GetPageContent(Uri uri)
        {
            //TODO: deal with clickable images
            Article article = new();
            article.Url = uri;
            HtmlDocument document = await GetDocument(uri);

            document?
                .DocumentNode?
                .Descendants()?
                .Where(n => n.Name == "script" || n.Name == "style")?.ToList()?
                .ForEach(n => n.Remove());

            foreach (HtmlNode link in document?.DocumentNode?.SelectNodes("//a[@href]"))
            {
                HtmlAttribute a = link?.Attributes["href"];

                if (a is not null)
                    a.Value = (new Uri(uri, a.Value)).ToString();
            }

            var head = document?.DocumentNode?.SelectSingleNode("//head");

            var image = head?.SelectSingleNode(".//meta[@property='og:image']")?.GetAttributeValue("content", null);

            if (image is not null && Uri.TryCreate(image, UriKind.Absolute, out Uri img))
                article.Image = img.ToString();

            article.Title = GetTitle(head);
            article.Author = head?
                .SelectSingleNode(".//meta[contains(@name, 'author') or contains(@property, 'author')]")?.GetAttributeValue("content", null);
            article.Description = head?
                .SelectSingleNode(".//meta[contains(@property, 'description') or contains(@name, 'description')]")?.GetAttributeValue("content", null);

            RemoveNodes(document);

            HtmlNode mainNode;

            if (document?.DocumentNode?.SelectSingleNode("//body//main") != null)
                mainNode = document?.DocumentNode?.SelectSingleNode("//body//main");
            else if (document?.DocumentNode?.SelectSingleNode("//body//article") != null)
                mainNode = document?.DocumentNode?.SelectSingleNode("//body//article");
            else
                mainNode = document?.DocumentNode?.SelectSingleNode("//body");

            StringBuilder html = new();
            StringBuilder text = new();

            var nodes = mainNode?.SelectNodes(".//p | .//h1 | .//h2 | .//h3 | .//h4 | .//h5 | .//pre |" +
                " .//h6 | .//img[@src | @data-src] | .//a[@href] | .//ol | .//ul | .//table | .//dl | .//code");

            if (nodes is not null)
            {
                foreach (var node in nodes)
                {
                    if (node is not null && html.ToString().Contains(Regex.Replace(node.OuterHtml, @"\n", " ").Trim()))
                        continue;

                    if (node is not null && node.Name.Equals("a") && node.InnerHtml is not null)
                        continue;

                    if (node is not null && node.Name.Equals("img") && !node.XPath.Contains("tbody"))
                    {
                        try
                        {
                            var imgUri = GetImageUri(uri, node);

                            if (text.Length > 0)
                                article.Content.Add(new TextElement(text.ToString(), ElementType.Text));
                            text.Clear();

                            if (imgUri.Scheme.Contains("http"))
                                article.Content.Add(new ImageElement(imgUri.ToString()));

                            html.Append(node.OuterHtml);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else if (node is not null && node.Name.Equals("table"))
                    {
                        if (text.Length > 0)
                            article.Content.Add(new TextElement(text.ToString(), ElementType.Text));
                        text.Clear();

                        article.Content.Add(new TextElement(node.OuterHtml, ElementType.Table));
                        html.Append(node.OuterHtml);
                    }
                    else if (node is not null && !node.XPath.Contains("nav") && !node.XPath.Contains("header") && !node.XPath.Contains("tbody"))
                    {
                        text.Append(Regex.Replace(node.OuterHtml, @"\n", " ").Trim());
                        html.Append(Regex.Replace(node.OuterHtml, @"\n", " ").Trim());
                    }
                }
            }

            if (text.Length > 0)
                article.Content.Add(new TextElement(text.ToString(), ElementType.Text));

            return article;
        }

        private string GetTitle(HtmlNode head)
        {
            string title;

            title = head?.SelectSingleNode(".//meta[contains(@property, 'og:title') or contains(@name, 'og:title')]")?.GetAttributeValue("content", null);

            if (title is null)
                title = head?.SelectSingleNode(".//title")?.InnerText;

            return title;
        }

        private Uri GetImageUri(Uri uri, HtmlNode node)
        {
            Uri imgUri;

            if (node.GetAttributeValue("data-src", null) is not null)
                imgUri = new Uri(uri, node.Attributes["data-src"].Value);
            else
                imgUri = new Uri(uri, node.Attributes["src"].Value);

            return imgUri;
        }

        private void RemoveNodes(HtmlDocument document)
        {
            List<string> xpaths = new();

            var nodesToDelete = document.DocumentNode?.SelectSingleNode(".//body")?
                .SelectNodes(
                    "//div[@aria-hidden = 'true'] | //span[@aria-hidden = 'true'] | //p[@aria-hidden = 'true'] | //aside | " +
                    "//svg[@aria-hidden = 'true'] | //a[@aria-hidden = 'true'] | //nav | //footer | //noscript | //div[@data-elementor-type]");

            if (nodesToDelete is not null)
            {
                foreach (var node in nodesToDelete)
                    xpaths.Add(node?.XPath);

                foreach (var xpath in xpaths)
                    document?.DocumentNode?.SelectSingleNode(xpath)?.Remove();
            }
        }

        private async Task<string> GetSource(Uri uri)
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla / 5.0 (Windows NT 6.3; WOW64; rv: 31.0) Gecko / 20100101 Firefox / 31.0");

            HttpResponseMessage response = await _httpClient.GetAsync(uri);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private async Task<HtmlDocument> GetDocument(Uri uri)
        {
            var source = await GetSource(uri);
            HtmlDocument document = new();
            document.LoadHtml(source);
            return document;
        }
    }
}