using RestApiTest.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestApiTest.Core.Models
{
    public class Comment : IVotable //?? czy podanie tutaj tego interfejsu nie wykracza już poza założenia modelu
    {
        public long Id { get; set; }
        public ForumUser Author { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime SentDate { get; set; }
        public DateTime? Modified { get; set; } //[Note] Trzeba mieć na uwadze, że metody post niekoniecznie będą aktualizować wszystko, tylko np. określone pole (jak status), w takim wypadku wartość powinna być nullowalna
        public bool Approved { get; set; }
        public IEnumerable<Comment> Responses { get; set; } //[Note]!!!!!: Do kolekcji reprezentujących zapytania na bazie najlepiej używać IQueryable -> IEnumerable jest optymalizowane dla odpytywania kolekcji w pamięci (wykonuje select'a po stronie serwera, a potem dopiero filtruje dane PO ZAŁADOWANIU PO STRONIE KLIENTA). IQueryable wykonuje pełne filtrowanie po stronie serwera. [source: https://www.c-sharpcorner.com/UploadFile/a20beb/ienumerable-vs-iqueryable-in-linq/]
        public bool IsRecommendedSolution { get; set; }
        public long Points { get; set; }
        public bool IsAdministrativeNote { get; set; }
        public BlogPost RelatedPost { get; set; }
        public ICollection<Vote> Votes { get; set; }

        public void AddVote(Vote voteToAdd)
        {
            throw new NotImplementedException();
        }

        public IVotable GetVotableObject()
        {
            return this;
        }

        public void RemoveReferenceToVote(long id)
        {
            Vote voteToRemove = Votes.FirstOrDefault(v => v.Id == id);
            if (voteToRemove != null)
            {
                Votes.Remove(voteToRemove);
            }
        }

        public void RemoveVote(long voteId)
        {
            throw new NotImplementedException();
        }

        //?? Jak jest ogarnięte po stronie framework'a to rozróżnienie IQueryable i IEnumerable? czy wystarczy rzeczywiśćie tylko zmienić typ interfejsu, żeby ta sama zmienna odwołująca się do kolekcji z bazy była przez EF przetwarzana zupełnie inaczej?

        //public long QuestionPostId { get; set; } //TODO: will work
        //public QuestionPost QuestionPostId {get; set;} //TODO: Will it work
    }
}