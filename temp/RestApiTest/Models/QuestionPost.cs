using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiTest.Models
{
    public class QuestionPost : BlogPost
    {
        public bool Approved { get; set; }
        public string ModeratorComment { get; set; }
        public List<Tag> Tags { get; set; }
        public bool IsSolved { get; set; }
    }
}
