using RestApiTest.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RestApiTest.Core.Models
{
    public class Comment : IVotable //?? czy podanie tutaj tego interfejsu nie wykracza już poza założenia modelu
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public ForumUser Author { get; set; }

        [Required, StringLength(50, MinimumLength = 2)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime SentDate
        {
            get;
            private set;
        }

        public DateTime Modified
        {
            get;
            private set;
        } //[Note] Trzeba mieć na uwadze, że metody post niekoniecznie będą aktualizować wszystko, tylko np. określone pole (jak status), w takim wypadku wartość powinna być nullowalna

        [Required]
        public bool Approved
        {
            get;
            private set;
        }
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

        public void UpdateDate()
        {
            Modified = DateTime.UtcNow;
        }

        public void Approve()
        {
            Approved = true;
        }

        public void SetInitialValues()
        {
            Approved = false;
            IsAdministrativeNote = false;
            IsRecommendedSolution = false;
            SentDate = Modified = DateTime.UtcNow;
            Points = 0;
        }

        //?? Jak jest ogarnięte po stronie framework'a to rozróżnienie IQueryable i IEnumerable? czy wystarczy rzeczywiśćie tylko zmienić typ interfejsu, żeby ta sama zmienna odwołująca się do kolekcji z bazy była przez EF przetwarzana zupełnie inaczej?

        //public long QuestionPostId { get; set; } //TODO: will work
        //public QuestionPost QuestionPostId {get; set;} //TODO: Will it work
    }
}