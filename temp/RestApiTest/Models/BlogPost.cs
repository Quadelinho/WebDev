using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiTest.Models
{
    public class BlogPost
    {
        [Required]
        public long Id { get; set; }
       
        public string Title { get; set; }
        public string Content { get; set; }

        [StringLength(25, MinimumLength = 3)]
        public string Author { get; set; }

        public DateTime Modified { get; set; } = DateTime.Now;
    }

    //TODO: baza SQL Express + database upgrade (ale już bez migracji)
    //TODO: dla bazy SQLite -> kolejna migracja z polem na datę typu string, żeby uwzględniało czas
    //TODO: NIP 6 i 7 + dalej
}
