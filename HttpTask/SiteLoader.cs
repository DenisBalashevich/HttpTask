using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using HtmlAgilityPack;

namespace HttpTask
{
    public class SiteLoader
    {
        private readonly ISiteSaver _siteSaver;
        private readonly List<Uri> _visitedPages;
        private readonly IList<ILimitation> _limitations;
        private readonly int _deepReferenceLevel;
        private readonly ILogger _logger;

        public SiteLoader(
            List<ILimitation> limitations,
            int deepReferenceLevel,
            ILogger logger,
            SiteSaver siteSaver
            )
        {
            _siteSaver = siteSaver;
            _visitedPages = new List<Uri>();
            _limitations = limitations;
            _deepReferenceLevel = deepReferenceLevel;
            _logger = logger;
        }

        public void LoadFromUrl(string url)
        {
            _visitedPages.Clear();
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(url);
                ScanUrl(httpClient, httpClient.BaseAddress, 0);
            }
        }

        private void ScanUrl(HttpClient httpClient, Uri uri, int level)
        {
            if (_visitedPages.Contains(uri) || IsLimitationExist(uri, level))
            {
                return;
            }
            _visitedPages.Add(uri);

            _logger.Log($"Process url: {uri}");
            var head = httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, uri)).Result;

            if (!head.IsSuccessStatusCode)
            {
                return;
            }

            ProcessHtmlDocument(httpClient, head, uri, level);
        }

        private void ProcessHtmlDocument(HttpClient httpClient, HttpResponseMessage response, Uri uri, int level)
        {
            _logger.Log($"Url founded: {uri}");

            var document = new HtmlDocument();
            document.Load(response.Content.ReadAsStreamAsync().Result, Encoding.UTF8);
            _logger.Log($"Html loaded: {uri}");

            var documentFileName = document.DocumentNode.Descendants("title").FirstOrDefault()?.InnerText + ".html";
            _siteSaver.SaveHtmlDocument(uri, documentFileName, GetDocumentStream(document));

            var attributesWithLinks = document.DocumentNode.Descendants().SelectMany(d => d.Attributes.Where(attribute => attribute.Name == "src" || attribute.Name == "href"));
            foreach (var attributesWithLink in attributesWithLinks)
            {
                ScanUrl(httpClient, new Uri(httpClient.BaseAddress, attributesWithLink.Value), level + 1);
            }
        }

        private bool IsLimitationExist(Uri uri, int deep)
        {
            return _limitations.Any(limitation => limitation.HasLimitation(uri)) || deep > _deepReferenceLevel;
        }

        private static Stream GetDocumentStream(HtmlDocument document)
        {
            var memoryStream = new MemoryStream();
            document.Save(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }
}
