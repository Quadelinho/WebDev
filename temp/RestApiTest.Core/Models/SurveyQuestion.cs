﻿namespace RestApiTest.Core.Models
{
    public class SurveyQuestion
    {
        public long Id { get; set; }
        public string Question { get; set; }
        //public List<string> AvailableResponses { get; set; }
        public string OpenResponse { get; set; }
        public ForumUser AnsweredBy { get; set; }
    }
}
