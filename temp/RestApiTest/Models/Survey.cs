using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiTest.Models
{
    public class Survey
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<SurveyQuestion> Questions { get; set; }
        public long TotalAnswers { get; set; }
    }
}
