using RestApiTest.Core.Interfaces;

namespace RestApiTest.Core.Models
{
    public class SurveyQuestion : IIdentifiable
    {
        public long Id { get; set; }
        public string Question { get; set; }
        //public List<string> AvailableResponses { get; set; }
        public string OpenResponse { get; set; }
        public ForumUser AnsweredBy { get; set; }

        public long GetIdentifier()
        {
            return Id;
        }
    }
}
