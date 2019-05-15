using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiTest.Models
{
    public class Statistics
    {
        public long RegisteredUsers { get; set; }
        public long InactiveUsers { get; set; }
        public long NewsletterSubscribers { get; set; }
        public long TotalQuestions { get; set; }
        public long SolvedQuestions { get; set; }
        public long NotApprovedQuestions { get; set; }
        public long TotalPosts { get; set; }
    }
}
