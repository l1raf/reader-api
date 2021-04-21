using HtmlAgilityPack;
using ReaderBackend.Scraper.Models;
using ReaderBackend.Scraper.Models.ArticleElements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReaderBackend.Scraper
{
    public class ArticleScraper : IArticleScraper
    {
        private static HttpClient HttpClient { get; set; } = new HttpClient();

        public async Task<Article> GetPageContent(Uri uri)
        {
            //TODO: deal with clickable images
            Article article = new();
            HtmlDocument document = await GetDocument(uri);

            document.DocumentNode.Descendants().Where(n => n.Name == "script" || n.Name == "style").ToList()
                .ForEach(n => n.Remove());

            foreach (HtmlNode link in document.DocumentNode.SelectNodes("//a[@href]"))
            {
                HtmlAttribute a = link.Attributes["href"];
                a.Value = (new Uri(uri, a.Value)).ToString();
            }

            article.Title = document?.DocumentNode?.SelectSingleNode("//title")?.InnerText;

            RemoveNodes(document);

            HtmlNode mainNode;

            if (document?.DocumentNode?.SelectSingleNode("//body//article") != null)
            {
                mainNode = document?.DocumentNode?.SelectSingleNode("//body//article");
            }
            else
            {
                mainNode = document?.DocumentNode?.SelectSingleNode("//body");
            }

            StringBuilder html = new();
            StringBuilder sb = new();

            var nodes = mainNode
                ?.SelectNodes(
                    ".//p | .//h1 | .//h2 | .//h3 | .//h4 | .//h5 | .//h6 | .//img[@src | @data-src] | .//a[@href] | .//ol | .//ul | .//table | .//dl");

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    if (node != null && html.ToString().Contains(Regex.Replace(node.OuterHtml, @"\n", " ").Trim()))
                        continue;

                    if (node != null && node.Name.Equals("img") && !node.XPath.Contains("tbody"))
                    {
                        try
                        {
                            var imgUri = GetImageUri(uri, node);

                            if (sb.Length > 0)
                                article.Content.Add(new TextElement(sb.ToString()));
                            sb.Clear();

                            if (imgUri.Scheme.Contains("http"))
                                article.Content.Add(new ImageElement(imgUri));

                            html.Append(node.OuterHtml);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else if (node != null && node.Name.Equals("table"))
                    {
                        if (sb.Length > 0)
                            article.Content.Add(new TextElement(sb.ToString()));
                        sb.Clear();

                        article.Content.Add(new TableElement(GetTable(node)));
                        html.Append(node.OuterHtml);
                    }
                    else if (node != null && !node.XPath.Contains("nav") && !node.XPath.Contains("header") && !node.XPath.Contains("tbody"))
                    {
                        sb.Append(Regex.Replace(node.OuterHtml, @"\n", " ").Trim());
                        html.Append(Regex.Replace(node.OuterHtml, @"\n", " ").Trim());
                    }
                }
            }

            if (sb.Length > 0)
                article.Content.Add(new TextElement(sb.ToString()));

            File.WriteAllText("html.txt", html.ToString());
            File.WriteAllText("html.html", html.ToString());

            return article;
        }

        private IEnumerable<IEnumerable<string>> GetTable(HtmlNode tableNode)
        {
            List<List<string>> table = new();

            foreach (var row in tableNode.SelectNodes(".//tr"))
            {
                List<string> rowParams = new();

                var headers = row.SelectNodes(".//th");

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        rowParams.Add($"<b>{header?.InnerHtml}</b>");
                    }
                }

                var rowElements = row.SelectNodes(".//td");

                if (rowElements != null)
                {
                    foreach (var rowData in rowElements)
                    {
                        rowParams.Add(rowData?.InnerHtml);
                    }
                }

                table.Add(rowParams);
            }

            return table;
        }

        private Uri GetImageUri(Uri uri, HtmlNode node)
        {
            Uri imgUri;

            if (node.GetAttributeValue("src", null) == null)
            {
                imgUri = new(uri, node.Attributes["data-src"].Value);
            }
            else
            {
                imgUri = new(uri, node.Attributes["src"].Value);
            }

            return imgUri;
        }

        private void RemoveNodes(HtmlDocument document)
        {
            List<string> xpaths = new();

            var nodesToDelete = document.DocumentNode?.SelectSingleNode(".//body")
                ?.SelectNodes(
                    "//div[@aria-hidden = 'true'] | //span[@aria-hidden = 'true'] | " +
                    "//svg[@aria-hidden = 'true'] | //a[@aria-hidden = 'true'] | //nav | //footer | //noscript");

            if (nodesToDelete != null)
            {
                foreach (var node in nodesToDelete)
                {
                    xpaths.Add(node?.XPath);
                }

                foreach (var xpath in xpaths)
                    document?.DocumentNode?.SelectSingleNode(xpath)?.Remove();
            }
        }

        private async Task<string> GetSource(Uri uri)
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();

            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla / 5.0 (Windows NT 6.3; WOW64; rv: 31.0) Gecko / 20100101 Firefox / 31.0");

            try
            {
                HttpResponseMessage response = await HttpClient.GetAsync(uri);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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