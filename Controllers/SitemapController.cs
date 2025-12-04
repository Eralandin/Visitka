using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml;

namespace Visitka.Controllers
{
    public class SitemapController : Controller
    {
        [Route("sitemap.xml")]
        public IActionResult Sitemap()
        {
            var sitemapNodes = new List<SitemapNode>
            {
                new SitemapNode {
                    Url = Url.Action("Index", "Home", null, Request.Scheme),
                    Priority = 1.0,
                    Frequency = SitemapFrequency.Daily
                },
                new SitemapNode {
                    Url = Url.Action("Index", "Portfolio", null, Request.Scheme),
                    Priority = 0.9,
                    Frequency = SitemapFrequency.Weekly
                },
                new SitemapNode {
                    Url = Url.Action("Index", "Price", null, Request.Scheme),
                    Priority = 0.8,
                    Frequency = SitemapFrequency.Weekly
                },
                new SitemapNode {
                    Url = Url.Action("Contacts", "Home", null, Request.Scheme),
                    Priority = 0.7,
                    Frequency = SitemapFrequency.Monthly
                },
                new SitemapNode {
                    Url = Url.Action("Index", "Request", null, Request.Scheme),
                    Priority = 0.8,
                    Frequency = SitemapFrequency.Weekly
                }
            };

            var xml = GenerateSitemapXml(sitemapNodes);
            return Content(xml, "application/xml", Encoding.UTF8);
        }

        private string GenerateSitemapXml(List<SitemapNode> nodes)
        {
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = false // Убедитесь, что декларация включена
            };

            using var stream = new MemoryStream();
            using var writer = XmlWriter.Create(stream, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

            foreach (var node in nodes)
            {
                writer.WriteStartElement("url");
                writer.WriteElementString("loc", node.Url);
                writer.WriteElementString("lastmod", node.LastModified.ToString("yyyy-MM-dd"));
                writer.WriteElementString("changefreq", node.Frequency.ToString().ToLower());
                writer.WriteElementString("priority", node.Priority.ToString("F1", System.Globalization.CultureInfo.InvariantCulture));
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();

            // Правильное преобразование потока в строку UTF-8
            stream.Position = 0;
            using var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }

    public class SitemapNode
    {
        public string Url { get; set; }
        public DateTime LastModified { get; set; } = DateTime.Now;
        public SitemapFrequency Frequency { get; set; } = SitemapFrequency.Monthly;
        public double Priority { get; set; } = 0.5;
    }

    public enum SitemapFrequency
    {
        Never,
        Yearly,
        Monthly,
        Weekly,
        Daily,
        Hourly,
        Always
    }
}