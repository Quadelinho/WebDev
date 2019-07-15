using Microsoft.EntityFrameworkCore;
using RestApiTest.Core.Models;

namespace RestApiTest.Infrastructure.Data
{
    //[Note] Jedynym punktem używającym kontekstu powinny być rzeczywiste implementacje Repository
    public class ForumContext : DbContext
    { //[Note] - Przyczyną było to, że ef core nie ogarnął takiej ilości zmian - trzeba było usunąć wszystkie migracje oraz snapshot kontekstu (pliki z projektu) i utworzyć je na nowo Wywołanie dotnet ef migrations add "nazwa" zwraca błąd, że nie jest w stanie utworzyć obiektu context
        public ForumContext(DbContextOptions<ForumContext> options) : base(options) //[Note] - przyczyną było to, że framework nie ogarnął większej ilości zmian, po usunięciu migracji i dodaniu na nowo początkowej migracji - Z jakiegoś powodu tworzenie migracji nie jest w stanie jeszcze ogarnąć tego kontekstu - tylko kilka klas jest tworzonych
        {

        }
        public DbSet<QuestionPost> Questions { get; set; }
        public DbSet<BlogPost> Posts { get; set; }
        public DbSet<ForumUser> Users { get; set; } //[NOTE]: Nazwy takie jak User są już często wykorzystane jako słowa kluczowe frameworka, dlatego trzeba dawać od razu inne (profilaktycznie)
        public DbSet<Comment> PostComments { get; set; }
        public DbSet<Vote> Votes { get; set; }
    }
}
