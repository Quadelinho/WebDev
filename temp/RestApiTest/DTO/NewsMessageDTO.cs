using System;

namespace RestApiTest.DTO
{
    public class NewsMessageDTO
    {
        public long? Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? PublishDate { get; set; }
    }
}
