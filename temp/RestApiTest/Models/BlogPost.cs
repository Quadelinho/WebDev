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
        public User Author { get; set; }

        //?? .net core nie obsługuje domyślnych wartości getter'ów
        //?? Czy da się jakoś oznaczyć pole, żeby nie było podawane w body? Czy w ogóle tak się robi, czy w praktyce się tego nie określa, 
            //a po prostu takie "automatyczne pola" jak np. modified po prostu i tak się zawsze nadpisuje z poziomu kodu?
        //?? Baza utworzona po przesiadce na SQLExpress'a dalej widzi tutaj typ datetime, nie string
        public DateTime? Modified { get; set; } //= DateTime.Now.ToLongDateString(); //?? Zmiana typu pola nie została wykryta przez komendę Update-Database jako modyfikacja do utowrzenia migracji
        public List<Comment> Comments { get; set; }
        public long Likes { get; set; }
        public string Dislikes { get; set; }
    }


    //TODO: Interfejs do repo (implementacja async)
    //TODO: Zastanowić się nad kolejnymi modelami, żeby to skomplikować (np. komentarze, oceny, autorzy, zapytania, artykuły, grafiki, ankiety, raporty i statystyki, subskrybenci, newsy)
    //[NOTE] - po zmianach w modelu trzeba wywołać komendę aktualizującą bazę -> albo z poziomu VS przez Package Manager Console wywołać 'Database-Update', albo uruchomić systemową konsolę , przejść do lokalizacji danego pliku csproj i wywołać komendę 'dotnet ef database update', albo podając opcję --project podać ścieżkę do pliku projektu
        //żeby powstała nowa migracja z określoną zmianą trzeba wywołać komendę: dotnet ef migrations add "nazwa migracji"

    //[DONE]: baza SQL Express + database upgrade (ale już bez migracji) //[NOTE]: cmd -> dotnet ef database update odtwarza bazę z migracji
    //[Done]: dla bazy SQLite -> kolejna migracja z polem na datę typu string, żeby uwzględniało czas  //Już uwzględnia czas, ale nie wykryło tej zmiany jako migracji
    //TODO: NIP 6 i 7 + dalej

}
