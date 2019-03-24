using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiTest.Models
{
    public class BlogPost
    {
        public long ID { get; set; }
       
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime Modified { get; set; }
    }
}
