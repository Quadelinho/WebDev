using RestApiTest.Core.Models;

namespace RestApiTest.DTO
{
    public class BlogPostDTO //[Note] - tak, stosuje się zazwyczaj przyrostek DTO lub Request - Czy używanie takiego przyrostka (DTO) jest zgodne z konwencją oficjalną? (Na necie takie widziałem).
    {
        public long? Id { get; set; } //?? Pola takie jak Id nie powinny być w DTO? Ogólnie, pola, których nie będzie się aktualizowało przez Update / Post nie mają być w DTO?
        public string Title {get; set;}
        public string Content { get; set; }
        public ForumUser Author { get; set; } //[Note] - raczej się nie daje id, tylko AutoMapper zastępuje typ referencyjny polami - Czy w DTO tak to powinno wyglądać dla minimalizowania danych, czy raczej powinna tu być referencja do obiektu User?
        //[Note] - podstawowa walidacja danych wejściowych jest na poziomie najwyższej warstwy (aplikacji), potem bardziej złożona walidacja pod kątem domenowym jest w warstwie service'ów (Warto tu użyć darmowej biblioteki FluentValidator, dzięki której łatwo też pisze się testy) Gdzie definiuje się obostrzenia co do danych (np. max długość stringa itp.) - czy nie powinno to też mieć odzwierciedlenia w DTO, tak jak w modelu?
        //?? Comments i Votes raczej nie są potrzebne w DTO BlogPost?
    }
}
