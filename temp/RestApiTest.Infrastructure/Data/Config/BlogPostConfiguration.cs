using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestApiTest.Core.Models;

namespace RestApiTest.Infrastructure.Data.Config
{
    public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
    {
        public void Configure(EntityTypeBuilder<BlogPost> builder)
        {
            builder.Property(b => b.Id).IsRequired();
            builder.Property(b => b.Title).IsRequired().HasMaxLength(400);
            builder.Property(b => b.Content).IsRequired();
            builder.Property(b => b.Author).HasMaxLength(25);
        }
    }
}
