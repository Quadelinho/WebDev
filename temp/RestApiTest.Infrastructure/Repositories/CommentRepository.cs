﻿using RestApiTest.Core.DTO;
using RestApiTest.Core.Exceptions;
using RestApiTest.Core.Interfaces.Repositories;
using RestApiTest.Core.Models;
using RestApiTest.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiTest.Infrastructure.Repositories
{
    public class CommentRepository : BaseRepository<Comment>, ICommentRepository //[Note] - konkretny, nie base, nawet gdyby miał być pusty - Czy w tym wypadku tak się robi w praktyce dla prostych repo, czy jednak powinien to być jakiś "konkretny" interfejs ICommentRepository?
    {
        //private ForumContext context;

        public CommentRepository(ForumContext context) : base(context)
        {
            //this.context = context;
        }

        public override async Task<Comment> AddAsync(Comment objectToAdd)
        {
            if(objectToAdd == null)
            {
                throw new BlogPostsDomainException("Failed to add comment - empty object");
            }

            objectToAdd.SetInitialValues();

            ForumUser author = await context.Users.FindAsync(objectToAdd.Author.Id);
            if(author != null && author.IsConfirmed)
            {
                objectToAdd.Author = author;
            }
            else
            {
                throw new AuthorNotFoundException("Comment cannot be applied because user does not exist or is not confirmed");
            }
            
            await context.PostComments.AddAsync(objectToAdd);
            await context.SaveChangesAsync();
            return objectToAdd;
        }

        public override async Task<Comment> ApplyPatchAsync(Comment objectToModify, List<PatchDTO> propertiesToUpdate)
        {
            var properties = propertiesToUpdate.ToDictionary(p => p.PropertyName, p => p.PropertyValue);
            if (properties.ContainsKey("Modified"))
            {
                throw new InvalidOperationException("Attempt to apply patch to restricted property");
            }

            var entityEntry = context.Entry(objectToModify);
            entityEntry.CurrentValues.SetValues(properties);
            entityEntry.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            entityEntry.Entity.UpdateDate();
            await context.SaveChangesAsync();
            return entityEntry.Entity;
        }

        public override async Task DeleteAsync(long id)
        {
            Comment comment = await context.PostComments.FindAsync(id);
            if(comment != null)
            {
                context.PostComments.Remove(comment);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IQueryable<Comment>> GetAllCommentsForPost(long commentedPostId)
        {
            BlogPost relatedPost = await context.Posts.FindAsync(commentedPostId); //?? czy to da radę znaleźć też obiekty klasy pochodnej? //TODO: Sprawdzić
            if(relatedPost == null)
            {
                throw new BlogPostsDomainException("Getting all post's comments failed - no post with given id exists");
            }

            return context.PostComments.Where(c => c.RelatedPost.Id == commentedPostId); //[Note] - konsola z EF dosyć dobrze działa, ale prawilnie może też być SQL profiler - jakim narzędziem najlepiej sprawdzać ile rzeczywiście leci strzałów na bazę (czy np. EF nie jest optymalizowany z lazy loadingiem tak, żeby to ogarnąć lepiej niż jawne wywołanie przez context?)
            //return relatedPost.Comments.AsQueryable(); //[Note] - lepiej na kontekście, żeby nie było dwukrotnie wykonywanych strzałów na bazę - Czy zwraca się tak, czy też powinienem wykonać operację na kontekście? Czy to sam framework ogarnie właśnie tak jak zrobiłem
        }

        public async Task<IQueryable<Comment>> GetAllCommentsForUser(long authorId)
        {
            ForumUser author = await context.Users.FindAsync(authorId); //[Note] - tak, trzeba coś dodać, bo inaczej nie ładuje od razu zależności (np. wywołanie author.UserComments zwraca null) sprawdzić czy nie jest potrzebny include, żeby zaciągnąć referencje
            if (author == null)
            {
                throw new BlogPostsDomainException("Getting all user's comments failed - no user with given id exists"); //[Note] - z kontrolera lepiej zwrócić NoContent W takich sytuacjach w praktyce rzuca się wyjątki, czy zwraca po prostu pustą kolekcję? //TODO: najlepiej zdefiniować wyjątek domenowy, że nie ma niczego do zwrócenia i obsługiwać to w global exception handler'ze zwracając NoContent
            }

            return context.PostComments.Where(c => c.Author.Id == authorId);
            //return author.UsersComments; //[Note] - odwołanie bezpośrednio do kontekstu działa od razu, bez koniecznonści osobnego definiowania ładowania zależności - Sprawdzić co będzie lepsze (sprawdzić query)
        }

        public override async Task<Comment> GetAsync(long id)
        {
            return await context.PostComments.FindAsync(id);
        }

        public override async Task<Comment> UpdateAsync(Comment objectToUpdate)
        {
            if (objectToUpdate == null)
            {
                throw new InvalidOperationException("Update failed - empty source object");
            }

            Comment comment = await context.PostComments.FindAsync(objectToUpdate.Id);
            if(comment != null)
            {
                objectToUpdate.UpdateDate();
                context.Entry(comment).CurrentValues.SetValues(objectToUpdate);
                await context.SaveChangesAsync();
                return comment;
            }
            else
            {
                throw new BlogPostsDomainException("Update comment failed - no object for update");
            }
            //Done: repo powinno zwracać queryable
        }

        public async Task<Comment> ApproveCommentAsync(long id)
        {
            Comment commentToApprove = await context.PostComments.FindAsync(id);
            if (commentToApprove != null)
            {
                commentToApprove.Approve();
                await context.SaveChangesAsync();
                return commentToApprove;
            }
            else
            {
                return null;
            }
        }

        public async Task<Comment> MarkCommentAsSolutionAsync(long id)
        {
            Comment commentToMark = await context.PostComments.FindAsync(id);
            if (commentToMark != null)
            {
                commentToMark.IsRecommendedSolution = true;
                await context.SaveChangesAsync();
                return commentToMark;
            }
            else
            {
                return null;
            }
        }
    }
}
