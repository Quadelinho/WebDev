using Microsoft.EntityFrameworkCore;
using RestApiTest.Core.Models;

namespace RestApiTest.Infrastructure.Data
{
    //[Note] Jedynym punktem używającym kontekstu powinny być rzeczywiste implementacje Repository
    public class ForumContext : DbContext
    {
        public ForumContext(DbContextOptions<ForumContext> options) : base(options) //TODO: ?? Z jakiegoś powodu tworzenie migracji nie jest w stanie jeszcze ogarnąć tego kontekstu - tylko kilka klas jest tworzonych
        {

        }
        public DbSet<QuestionPost> Questions { get; set; }
        public DbSet<BlogPost> Posts { get; set; }
        public DbSet<ForumUser> Users { get; set; } //[NOTE]: Nazwy takie jak User są już często wykorzystane jako słowa kluczowe frameworka, dlatego trzeba dawać od razu inne (profilaktycznie)
        public DbSet<Comment> PostComments { get; set; }
        public DbSet<Vote> Votes { get; set; }

        //TODO: na razie zrobić tylko 2 i kontrolery (Question, Comment kontroler)
    }
}
