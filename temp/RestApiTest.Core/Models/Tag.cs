namespace RestApiTest.Core.Models
{
    public class Tag
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public BlogPost RelatedPost { get; set; }
    }
}
