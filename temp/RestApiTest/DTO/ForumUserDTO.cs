using System;

namespace RestApiTest.DTO
{
    public class ForumUserDTO
    {
        public long? Id { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public DateTime? RegisteredSince { get; set; }
        public DateTime? LastLoggedIn { get; set; }
        public int? ReputationPoints { get; set; }
        public bool? IsSubscribedToNewsletter { get; set; }
        //TODO: Dodać kolekcję komentarzy użytkownika do DTO
    }
}
