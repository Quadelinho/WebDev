using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiTest.Core.Models
{
    public class QuestionPost : BlogPost
    {
        public bool? Approved { get; set; }
        //public string ModeratorComment { get; set; } //TODO: [Done] Przerobić na encję Comment z flagą moderator
        public IQueryable<Tag> Tags { get; set; } //[Note] - używać odpowiednich interfejsów kolekcji //?? Czy skoro IQueryable jest optymalizowane pod kątem filtrowania i pobierania danych z bazy, to znaczy, że w praktyce wszystkie kolekcje w modelu powinny być jako IQueryable?

        //TODO: IQueryable nie powinno być na poziomie modelu, tylko w implementacji Repo
        public bool? IsSolved { get; set; } 
    }
}

//TODO: start using DTO