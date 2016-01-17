using System.Collections.Generic;
using Umbraco.Core.Models;

namespace OldLima.Models
{
    public class InfoWidgetVM
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public IEnumerable<IPublishedContent> WidgetItems { get; set; }

        public string Carousel { get; set; }
    }
}