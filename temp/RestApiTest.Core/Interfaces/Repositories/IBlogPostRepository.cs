﻿using RestApiTest.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Linq;
using System;
using RestApiTest.Core.Interfaces.Repositories;

namespace RestApiTest.Core.Interfaces.Repositories
{
    public interface IBlogPostRepository : IBaseRepository<BlogPost>, ICommentable, IMarkable
    {
        //[Note] - bo tego typu rzeczy nie daje się w interfejsach - to już by wymuszało konkretną implementację asynchroniczną - Dlaczego nie jest tu w ogóle widoczny ten interfejs IAsyncEnumerable, chociaż w Infrastructure działa ok?
        //[Note] - bo ma taski zaimplementowane wewnętrznie - Dlaczego IAsyncEnumerable nie jest przekazywany do Task'a (przykład z NIP: https://github.com/wi7a1ian/nip-lab-2018/blob/68b5be9f93b435e537344c5e6a59b8ed0db7f830/src/Nip.Blog/Services/Posts/Posts.API/Repositories/IBlogPostRepository.cs)
            //Czy temu, że IAsyncEnumerable pozwala zwracać wyniki na bieżąco, używając yield'a?
        Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync(); //[Note] - ma sens jeśli nie chcę czegoś udostępniać wszędzie, ale równie dobrze taka metoda mogłaby być też w interfejsie IRepo, ale nie wystawiana poza repozytorium, gdybym nie chciał jej używać (przy czym wtedy mam pisany kod, który z założenia nie będzie używany) - Czy taka forma ma sens, bo wygląda to na trochę bzdurny interfejs. Czy warto w ogóle dodać np. interface rozszerzający IBase o metodę GetAll?
    }
}
