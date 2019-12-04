using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using RestApiTest.Core.Models;
using RestApiTest.Infrastructure.Data;

namespace RestApiTest.Services.Validators
{
    public class BlogPostValidator : AbstractValidator<BlogPost>
    {
        //TODO: testy dla FluentValidator'a
        //TODO: validator raczej do DTO, niż do modeli

        //[Note] - nie powinien się odwoływać bezpośrednio do kontekstu, tylko korzystać z metod wystawionych przez repo - Czy ten walidator z samej swojej ideii nie narusza zasad architektury onion, zwaszcza zdefiniowany w projekcie Services - bo wymusza zależność od core i infrastructure?
        private ForumContext dataContext;
        public BlogPostValidator(ForumContext dataContext) //todo: usunąć context i zrobić tylko prostą walidację
        {
            this.dataContext = dataContext;
            //RuleFor(post => post.Title).;
            //[Note] - raczej nie, to powinny być względnie proste reguły - Czy we FluentValidation używa się złożonej logiki jak np. kontakt z bazą i sprawdzenie, czy dany tytuł nie jest zduplikowany? 
               //Czy może to powinna być moja klasa walidująca, a fluent tylko do podstawowej walidacji? 
               //[Note] - Fluent validator pozwala na bardziej rozbudowane scenariusze, których nie dało by się pokryć samymi atrybutami, jak np. wzajemne zależności poszczególnych pól (np. jeśli pole 1 jest puste to sprawdź, czy pole 2 też jest puste) - Jaka jest przewaga fluent validatora (w podstawowym zakresie) nad atrybutami jak np. required, czy maxlength?
        }

        public override async Task<ValidationResult> ValidateAsync(ValidationContext<BlogPost> context, CancellationToken cancellation = default(CancellationToken))
        {
            ValidationResult result = null;
            result = await base.ValidateAsync(context, cancellation);
            if(result.IsValid)
            {
                if (!ValidatePostTitleDuplicates())
                {
                    result = new ValidationResult(new List<ValidationFailure>() { new ValidationFailure("Title", "Given title already exists")});
                }
            }

            return result;
        }

        private bool ValidatePostTitleDuplicates()
        {
            bool isTitleDuplicated = true;

            return isTitleDuplicated;
        }
    }
}
