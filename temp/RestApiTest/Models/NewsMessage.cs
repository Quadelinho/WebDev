using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiTest.Models
{
    public class NewsMessage
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<Tag> Tags { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime? ValidTill { get; set; }
    }
}
