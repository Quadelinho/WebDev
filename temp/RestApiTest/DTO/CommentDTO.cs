using System;

namespace RestApiTest.DTO
{
    public class CommentDTO
    {
        public long? Id { get; set; }
        public long? AuthorId { get; set; }
        //public ForumUserDTO Author { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        //public DateTime? SentDate { get; set; }
        //public long? Points { get; set; } //[Note] - raczej nie, bo to już podchodzi pod logikę biznesową - wartości wyliczanych nie daje  Czy to ma być w DTO?
    }
}
