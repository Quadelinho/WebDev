using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiTest.Models
{
    public class User
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
    }
}
