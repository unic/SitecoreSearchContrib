using Sitecore.Data.Fields;
using Sitecore.Search.Crawlers.FieldCrawlers;

namespace scSearchContrib.Crawler.FieldCrawlers
{
    /// <summary>
    /// The checkbox field crawler.
    /// </summary>
    public class CheckboxFieldCrawler : FieldCrawlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckboxFieldCrawler" /> class.
        /// </summary>
        /// <param name="field">The field.</param>
        public CheckboxFieldCrawler(Field field) : base(field) { }

        /// <summary>
        /// Returns true/false as textstring for checkbox field values
        /// </summary>
        /// <returns>string "true" or "false" (instead of "0" or "1"</returns>
        public override string GetValue()
        {
            return this._field.Value == "1" ? "true" : "false";
        }
    }
}
