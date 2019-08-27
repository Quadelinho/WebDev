using Microsoft.EntityFrameworkCore;
using RestApiTest.Core.DTO;
using RestApiTest.Core.Exceptions;
using RestApiTest.Core.Interfaces;
using RestApiTest.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiTest.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class, IIdentifiable
    {
        //protected ForumContext context;
        protected DbContext context;
        public BaseRepository(DbContext context)
        {
            this.context = context;
        }

        public virtual async Task<T> AddAsync(T objectToAdd, Action additionalSteps = null)
        {
            if (objectToAdd == null)
            {
                throw new BlogPostsDomainException("Failed to add comment - empty object");
            }

            //[Note] - tak, nic nie stoi na przeszkodzie i korzysta się z tego w praktyce - Czy stosuje się w praktyce przekazywanie do generycznego bazowego repo akcji / delegatów pozwalających definiować np. dodatkowe przypisania warotści poza tym co jest w metodzie bazowej?
            additionalSteps?.Invoke();
            await context.Set<T>().AddAsync(objectToAdd);
            await context.SaveChangesAsync();
            return objectToAdd;
        }
        
        public virtual Task<T> ApplyPatchAsync(T objectToModify, List<PatchDTO> propertiesToUpdate)
        {
            throw new NotImplementedException();
        }

        public virtual async Task DeleteAsync(long id, Action additionalPreSteps = null)
        {
            var objectToRemove = await context.Set<T>().FindAsync(id);
            if (objectToRemove != null)
            {
                additionalPreSteps?.Invoke();
                context.Set<T>().Remove(objectToRemove);
                await context.SaveChangesAsync();
            }
        }

        public virtual Task<T> GetAsync(long id)
        {
            throw new NotImplementedException();
        }

        protected virtual long GetKey<T>(T entity)
        {
            var keyName = context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties
                .Select(x => x.Name).Single();

            return (long)entity.GetType().GetProperty(keyName).GetValue(entity, null);
        }

        public virtual async Task<T> UpdateAsync(T objectToUpdate, Action additionalPreSteps = null)
        {
            if (objectToUpdate == null)
            {
                throw new InvalidOperationException("Update failed - empty source object");
            }

            long id = GetKey<T>(objectToUpdate); //Przykład użycia refleksji do wydobycia klucza głównego w sposób generyczny

            additionalPreSteps?.Invoke();
            //TODO: Done - [Note] - można przez refleksję dostać się do wszystkich typów kluczy, prywatnego, obcego i złożonych - sprawdzić czy nie istnieje sposób na pozyskanie informacji o primary key (źródło: https://stackoverflow.com/questions/30688909/how-to-get-primary-key-value-with-entity-framework-core)
            T entityToModify = await context.Set<T>().FindAsync(objectToUpdate.GetIdentifier()); //[Note] - najczęściej używa się interfejsu lub dziedziczy po wspólnej klasie bazowej mającej publiczny akcesor, ale da się też refleksją - Czy tu jest jakiś sprytny sposób, żeby z typu generycznego wyłuskać propery klucza do find'a?
                                                                                          //Czy tylko refleksja, lub klasa bazowa / interfejs? //[Note] - najczęściej stosuje się interfejs lub klasę bazową dla wszystkich repo. Ewentualnie można przekazywać jako parametr do tej metody predykat, który ma być użyty przez Linq do znalezienia właściwego obiektu
            if (entityToModify != null)
            {
                context.Entry<T>(entityToModify).CurrentValues.SetValues(objectToUpdate); //[Note] !! - To nie ogarnia zagnieżdżonych typów referencyjnych, tylko proste. Jeśli properties'y są referencjami, trzeba je zaktualizować indywidualnie (https://stackoverflow.com/questions/13236116/entity-framework-problems-updating-related-objects)
                await context.SaveChangesAsync(); //?? Było polecane użycie update, żeby nie zmieniać całego kontekstu, ale nie ma update'u asynchronicznego
                return entityToModify;//[Note] - jeśli było by ryzyko, że będzie jednoczesna aktualizacja na bazie, to powinno się odczytywać zawsze z bazy - Czy wystarczy, że zwrócę obiekt post, czy muszę go ponownie odczytywać z bazy, żeby wykluczyć jakiekolwiek niespójności (powinno raczej wystarczyć zwrócenie tego obiektu, bo context powinien być spójny z bazą)?
            }
            else
            {
                throw new BlogPostsDomainException("Update failed - no post to update");
            }
        }
    }
}
