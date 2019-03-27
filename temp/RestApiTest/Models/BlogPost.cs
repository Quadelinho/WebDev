using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiTest.Models
{
    public class BlogPost
    {
        [Required]
        public long Id { get; set; }
       
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime Modified { get; set; }
    }
}
