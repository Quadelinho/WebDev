namespace RestApiTest.DTO
{
    public class VoteDTO
    {
        public bool? IsLike { get; set; }
        public long? VotedPostId { get; set; }
        public long? VotedCommentId { get; set; }
        public long? VoterId { get; set; }
    }
}
