using System.Collections.Generic;

namespace RestApiTest.Core.Models
{
    public class Tag
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<BlogPost> RelatedPosts { get; set; }
    }
}
