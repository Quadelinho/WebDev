using RestApiTest.Core.Models;
using System.Collections.Generic;

namespace RestApiTest.DTO
{
    public class BlogPostDTO //[Note] - tak, stosuje się zazwyczaj przyrostek DTO lub Request - Czy używanie takiego przyrostka (DTO) jest zgodne z konwencją oficjalną? (Na necie takie widziałem).
    {
        public long? Id { get; set; } //[Note] - miusi być, żeby było wiadomo co aktualizować - Pola takie jak Id nie powinny być w DTO?
        public string Title {get; set;}
        public string Content { get; set; }
        //[Note] - tak, bo jedną z ról DTO jest ukrywanie rzeczywistej implementacji - Czy zamiast typu referencyjnego mam w obiekcie DTO wstawiać też obiekt DTO (np. ForumUserDTO)?
        public ForumUserDTO Author { get; set; } //[Note] - raczej się nie daje id, tylko AutoMapper zastępuje typ referencyjny polami - Czy w DTO tak to powinno wyglądać dla minimalizowania danych, czy raczej powinna tu być referencja do obiektu User?
        //[Note] - podstawowa walidacja danych wejściowych jest na poziomie najwyższej warstwy (aplikacji), potem bardziej złożona walidacja pod kątem domenowym jest w warstwie service'ów (Warto tu użyć darmowej biblioteki FluentValidator, dzięki której łatwo też pisze się testy) Gdzie definiuje się obostrzenia co do danych (np. max długość stringa itp.) - czy nie powinno to też mieć odzwierciedlenia w DTO, tak jak w modelu?
        //[Note] - Są potrzebne, bo mają być tak samo przetwarzane przez API - Comments i Votes raczej nie są potrzebne w DTO BlogPost?
        public IEnumerable<CommentDTO> Comments { get; set; }
        public IEnumerable<VoteDTO> Votes { get; set; }
    }

    //todo: change DTOs to request/response (e.g. postUpdateRequest)
    //todo: folders change to domain (e.g. posts with postController, postValidator etc)
}