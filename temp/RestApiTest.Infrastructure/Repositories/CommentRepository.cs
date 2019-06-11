using RestApiTest.Core.Exceptions;
using RestApiTest.Core.Interfaces.Repositories;
using RestApiTest.Core.Models;
using RestApiTest.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestApiTest.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository //IBaseRepository<Comment> //?? Czy w tym wypadku tak się robi w praktyce dla prostych repo, czy jednak powinien to być jakiś "konkretny" interfejs ICommentRepository?
    {
        private ForumContext context;

        public CommentRepository(ForumContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(Comment objectToAdd)
        {
            if(objectToAdd == null)
            {
                throw new BlogPostsDomainException("Failed to add comment - empty object");
            }

            await context.PostComments.AddAsync(objectToAdd);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            Comment comment = await context.PostComments.FindAsync(id);
            if(comment != null)
            {
                context.PostComments.Remove(comment);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Comment>> GetAllCommentsForPost(long commentedPostId)
        {
            BlogPost relatedPost = await context.Posts.FindAsync(commentedPostId); //?? czy to da radę znaleźć też obiekty klasy pochodnej?
            if(relatedPost == null)
            {
                throw new BlogPostsDomainException("Getting all post's comments failed - no post with given id exists");
            }

            return relatedPost.Comments; //?? Czy zwraca się tak, czyyteż powinienem wykonać operację na kontekście? Czy to sam framework ogarnie właśnie tak jak zrobiłem
        }

        public async Task<IEnumerable<Comment>> GetAllCommentsForUser(long authorId)
        {
            ForumUser author = await context.Users.FindAsync(authorId);
            if (author == null)
            {
                throw new BlogPostsDomainException("Getting all user's comments failed - no user with given id exists"); //?? W takich sytuacjach w praktyce rzuca się wyjątki, czy zwraca po prostu pustą kolekcję?
            }

            return author.UsersComments;
        }

        public async Task<Comment> GetAsync(long id)
        {
            return await context.PostComments.FindAsync(id);
        }

        public async Task UpdateAsync(Comment objectToUpdate)
        {
            if (objectToUpdate == null)
            {
                throw new InvalidOperationException("Update failed - empty source object");
            }

            Comment comment = await context.PostComments.FindAsync(objectToUpdate.Id);
            if(comment != null)
            {
                context.Entry(comment).CurrentValues.SetValues(objectToUpdate);
                await context.SaveChangesAsync();
            }
            else
            {
                throw new BlogPostsDomainException("Update comment failed - no object for update");
            }
        }
    }
}
