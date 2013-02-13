using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Search.Crawlers.FieldCrawlers;
using scSearchContrib.Searcher.Utilities;

namespace scSearchContrib.Crawler.FieldCrawlers
{
    using scSearchContrib.Crawler.Utilities;

    /// <summary>
    /// PdfMediaFieldCrawler extends FieldCrawlerBase from the Sitecore.Search 
    /// API to provide text streams of a given PDF document.
    /// </summary>
    public class PdfMediaFieldCrawler : FieldCrawlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfMediaFieldCrawler" /> class.
        /// </summary>
        /// <param name="field">The field.</param>
        public PdfMediaFieldCrawler(Field field) : base(field) { }

        /// <summary>
        /// Extracts text from pdf and returns that text
        /// </summary>
        /// <returns>Text of a pdf as string</returns>
        public override string GetValue()
        {
            if (_field.Item.Paths.IsMediaItem)
            {
                MediaItem mediaItem = _field.Item;
                if (mediaItem != null && mediaItem.MimeType.Equals(PdfUtil.PdfMimetype))
                {
                    return PdfUtil.ParsePdf(mediaItem);
                }
            }

            return string.Empty;
        }
    }
}
