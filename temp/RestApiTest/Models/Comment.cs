using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiTest.Models
{
    public class Comment
    {
        public long Id { get; set; }
        public User Author { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime SentDate { get; set; }
        public DateTime? Modified { get; set; }
        public bool Approved { get; set; }
        public List<Comment> Responses { get; set; }
        public bool IsRecommendedSolution { get; set; }
        public long Points { get; set; }
    }
}
