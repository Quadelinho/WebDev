using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RestApiTest.Models;

namespace RestApiTest.Data
{
    public class BlogDBContext : DbContext
    {
        public BlogDBContext(DbContextOptions<BlogDBContext> options) :base(options)
        {

        }
        public DbSet<BlogPost> BlogPosts { get; set; }
    }
}
