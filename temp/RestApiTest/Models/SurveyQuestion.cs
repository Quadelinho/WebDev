using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiTest.Models
{
    public class SurveyQuestion
    {
        public long Id { get; set; }
        public string Question { get; set; }
        //public List<string> AvailableResponses { get; set; }
        public string OpenResponse { get; set; }
        public User AnsweredBy { get; set; }
    }
}
