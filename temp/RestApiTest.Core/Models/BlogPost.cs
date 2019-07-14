using RestApiTest.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RestApiTest.Core.Models
{
    public class BlogPost : IVotable /*: IMarkable*/ //?? Czy tego typu implementacje powinny być w ramach klasy modelu, czy raczej implementacji repozytorium (BlogPostRepository)?
    {
        [Required]
        public long Id { get; set; }
       
        [Required, StringLength(400, MinimumLength = 5)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [StringLength(25, MinimumLength = 3)]
        //[Required] //Wykomentowane na potrzeby prostego tworzenia początkowych danych testowych
        public ForumUser Author { get; set; } //[Note] AutoMapper jest w stanie to ogarnąć, jeśli ma zdefiniowane w konfiguracji mapowanie obiektu tej klasy - wtedy w odpowiednim mapowanym obiekcie (np. BlogPostDTO) z automatu zamieniłby referencję do ForumUsers na właściwości określone w obiekcie ForumUserDTO
                                                //TODO: Sprawdzić, czy jeśli AutoMapper nie miałby zdefiniowanego mapowania na ForumUserDTO (ani nic w ogóle dla ForumUser, to czy wstawiłby tu właściwości klasy ForumUser?
                                                //TODO: Sprawdzić, jak by się zachował przy takim automatycznym mapowaniu typów zagnieżdżonych, gdyby taki typ wewnętrzny miał właściwość o takiej samej nazwie jak nadrzędny (np. Title z posta i Title z Comment)?
                                                //[Note - potencjalnie trzeba skonfigurować - sprawdzić.] Skoro AutoMapper ogarnia z automatu zmienne referencyjne wstawiając pola, to co zrobi dla listy obiektów (np. Comments)?

        //[Note] .net core nie obsługuje domyślnych wartości getter'ów
        //[Note] - rozwiązuje się to sotosując wzorzec DTO (Data Transfer Objects), żeby uproszczone obiekty z nullowalnymi właściwościami mogły być definiowane do przekazywania minimalnej ilości niezbędnych danych - Czy da się jakoś oznaczyć pole, żeby nie było podawane w body? Czy w ogóle tak się robi, czy w praktyce się tego nie określa, 
            //a po prostu takie "automatyczne pola" jak np. modified po prostu i tak się zawsze nadpisuje z poziomu kodu?
        public DateTime Modified //{ get; set; } //= DateTime.Now.ToLongDateString(); //?? Zmiana typu pola nie została wykryta przez komendę Update-Database jako modyfikacja do utowrzenia migracji
        {
            get; //TODO: [DONE, z użyciem private set] ustawianie Modified zrobić na triggerach w bazie i getter'ami zawsze zwracać aktualny stan z bazy //[Note] - bezpośrednio w migracjach raczej nie, trzeba to osobno manualnie zdefiniować - Czy tego typu elementy jak triggery da się też odzwierciedlić w tych plikach tworzących migracje bazy? 
            private set;
        }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Vote> Votes { get; set; } //TODO: przerobić like'i na Event Sourcing. Przerobić na model Vote (tabela z poszczególnymi informacjami [Note] - pozwala w razie czego odtworzyć stan np.

        //public void AddVote(Vote voteToAdd)
        //{
        //    //Votes.
        //}

        public long GetId()
        {
            return Id;
        }

        public IVotable GetVotableObject()
        {
            return this;
        }

        public void RemoveReferenceToVote(long id)
        {
            Vote voteToRemove = Votes.FirstOrDefault(v => v.Id == id);
            if(voteToRemove != null)
            {
                Votes.Remove(voteToRemove);
            }
        }

        public void UpdateModifiedDate()
        {
            Modified = DateTime.UtcNow;
        }

        //public void RemoveVote(long voteId)
        //{
        //    throw new NotImplementedException();
        //}
    } //


    //TODO: Pozostałe interfejsy do repo (implementacja async)
    //[Done] Zastanowić się nad kolejnymi modelami, żeby to skomplikować (np. komentarze, oceny, autorzy, zapytania, artykuły, grafiki, ankiety, raporty i statystyki, subskrybenci, newsy)
    //[NOTE] - po zmianach w modelu trzeba wywołać komendę aktualizującą bazę -> albo z poziomu VS przez Package Manager Console wywołać 'Database-Update', albo uruchomić systemową konsolę , przejść do lokalizacji danego pliku csproj i wywołać komendę 'dotnet ef database update', albo podając opcję --project podać ścieżkę do pliku projektu
        //żeby powstała nowa migracja z określoną zmianą trzeba wywołać komendę: dotnet ef migrations add "nazwa migracji"
    //[DONE]: baza SQL Express + database upgrade (ale już bez migracji) //[NOTE]: cmd -> dotnet ef database update odtwarza bazę z migracji
    //[Done]: dla bazy SQLite -> kolejna migracja z polem na datę typu string, żeby uwzględniało czas  //Już uwzględnia czas, ale nie wykryło tej zmiany jako migracji

}
