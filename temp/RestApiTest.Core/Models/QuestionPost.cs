using System.Collections.Generic;

namespace RestApiTest.Core.Models
{
    public class QuestionPost : BlogPost
    {
        public bool? Approved { get; set; }
        //public string ModeratorComment { get; set; } //TODO: [Done] Przerobić na encję Comment z flagą moderator
        public IEnumerable<Tag> Tags { get; set; } //[Note] - używać odpowiednich interfejsów kolekcji, zależnie od tego co chce się zasugerować użytkownikowi interfejsu (np. jeśli chcemy przekazać, że wyniki będzie można/trzeba dodatkowo filtrować zapytaniami, to używa się IQueryable itd.) - - Czy skoro IQueryable jest optymalizowane pod kątem filtrowania i pobierania danych z bazy, to znaczy, że w praktyce wszystkie kolekcje w modelu powinny być jako IQueryable?

        public bool IsSolved { get; set; } 
    }
}