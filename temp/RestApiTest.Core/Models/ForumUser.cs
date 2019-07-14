using System;
using System.Collections.Generic;

namespace RestApiTest.Core.Models
{
    public class ForumUser
    {
        public long Id { get; set; }
        public Roles Role { get; private set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public DateTime RegisteredSince { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime LastLoggedIn { get; set; }
        public int ReputationPoints { get; set; }
        public bool SubscribedToNewsletter { get; set; }
        public ICollection<Comment> UserComments { get; set; }
    }
}
