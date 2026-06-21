using System;
using System.Collections.Generic;

namespace NewsMonitor.Models
{
    // Одна новость со всеми полями из задания
    public class NewsItem
    {
        public string Title { get; set; } = "";
        public string Url { get; set; } = "";
        public DateTime? Published { get; set; }
        public string Summary { get; set; } = "";
        public List<string> Tags { get; set; } = new List<string>();
        public string Source { get; set; } = "";
    }
}
