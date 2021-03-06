﻿namespace RssService.Business.Entities
{
    public class RssItem : BaseEntity
    {
        public string RssUrl { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Summary { get; set; }
        public string RssId { get; set; }
        public string Link { get; set; }
    }
}