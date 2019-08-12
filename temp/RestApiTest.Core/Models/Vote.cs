using RestApiTest.Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace RestApiTest.Core.Models
{
    public class Vote : IIdentifiable
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public bool IsLike { get; set; }

        public BlogPost VotedPost { get; set; } // Jak EF ogarnia w takiej sytuacji Liskov'a - jeśli jako post przypiszę obiekt klasy pochodnej? Tam zdaje się były te 3 strategie reprezentacji dziedziczenia w tabelach (jedna tabela podstawowa, a druga przechowująca tylko pola klasy pochodnej i id powiązanego rekordu w tabeli bazowej albo redundancja danych, albo jeszcze jakieś 3 podejście)
        public Comment VotedComment { get; set; }

        [Required]
        public ForumUser Voter { get; set; }
        public DateTime Modified { get; set; }

        public long GetIdentifier()
        {
            return Id;
        }
    }
}
//Interface: DeleteVote, RegisterVote, UpdateVote, GetVoterName/Details, GetVotedElement
