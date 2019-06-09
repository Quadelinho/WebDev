using System.Collections.Generic;
using System.Linq;

namespace RestApiTest.Core.Models
{
    public class Survey
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IEnumerable<SurveyQuestion> Questions { get; set; }
        public long TotalAnswers { get; set; }
    }
}
