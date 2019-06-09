using System;
using System.Collections.Generic;

namespace RestApiTest.Core.Models
{
    public class NewsMessage
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime? ValidTill { get; set; }
    }
}
