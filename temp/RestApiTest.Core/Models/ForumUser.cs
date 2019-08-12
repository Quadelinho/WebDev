using RestApiTest.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestApiTest.Core.Models
{
    public class ForumUser : IIdentifiable
    {
        [Required]
        public long Id { get; set; }
        public Roles Role { get; private set; }

        [Required, StringLength(200, MinimumLength = 8)]
        public string Email { get; set; }

        [Required, StringLength(50, MinimumLength = 3)]
        public string Login { get; set; }

        [Required, StringLength(200, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        public DateTime RegisteredSince
        {
            get;
            private set;
        }

        [Required]
        public bool IsConfirmed
        {
            get;
            private set;
        }

        public DateTime? LastLoggedIn { get; set; }
        public int ReputationPoints { get; set; }
        public bool SubscribedToNewsletter { get; set; }
        public ICollection<Comment> UserComments { get; set; }

        //public void SetUserRegistrationDate()
        //{
        //    RegisteredSince = DateTime.UtcNow;
        //}

        public void Confirm()
        {
            IsConfirmed = true;
        }

        public long GetIdentifier()
        {
            return Id;
        }

        public void SetInitialValues()
        {
            IsConfirmed = false;
            SubscribedToNewsletter = false;
            RegisteredSince = DateTime.UtcNow;
        }
    }
}
