using System;
using System.Collections.Generic;

namespace RestApiTest.DTO
{
    public class ForumUserDTO
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public DateTime? RegisteredSince { get; set; }
        public DateTime? LastLoggedIn { get; set; }
        public int? ReputationPoints { get; set; }
        public bool SubscribedToNewsletter { get; set; }
        public IEnumerable<CommentDTO> UserComments { get; set; } //[Note] - tak, wszystko DTO - Typy referencyjne w DTO powinny wskazywać na typ DTO, a nei rzeczywisty, tak?
    }
}
