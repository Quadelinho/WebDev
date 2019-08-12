using RestApiTest.Core.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestApiTest.Core.Models
{
    public class Tag : IIdentifiable
    {
        [Required]
        public long Id { get; set; }

        [Required, StringLength(20)]
        public string Name { get; set; }
        public IEnumerable<BlogPost> RelatedPosts { get; set; }

        public long GetIdentifier()
        {
            return Id;
        }
    }
}
