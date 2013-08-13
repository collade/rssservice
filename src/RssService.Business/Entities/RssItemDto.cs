namespace RssService.Business.Entities
{
    using System.Runtime.Serialization;

    [DataContract]
    public class RssItemDto
    {
        [DataMember]
        public string Link { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Content { get; set; }
        [DataMember]
        public string Time { get; set; }
    }
}