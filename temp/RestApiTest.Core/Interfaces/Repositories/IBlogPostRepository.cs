using RestApiTest.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Linq;
using System;
using RestApiTest.Core.Interfaces.Repositories;

namespace RestApiTest.Core.Interfaces.Repositories
{
    public interface IBlogPostRepository : IBaseRepository<BlogPost>, ICommentable, IMarkable
    {
        //?? Dlaczego nie jest tu w ogóle widoczny ten interfejs IAsyncEnumerable, chociaż w Infrastructure działa ok?
        //?? Dlaczego IAsyncEnumerable nie jest przekazywany do Task'a (przykład z NIP: https://github.com/wi7a1ian/nip-lab-2018/blob/68b5be9f93b435e537344c5e6a59b8ed0db7f830/src/Nip.Blog/Services/Posts/Posts.API/Repositories/IBlogPostRepository.cs)
            //Czy temu, że IAsyncEnumerable pozwala zwracać wyniki na bieżąco, używając yield'a?
        /*IAsyncEnumerable*/Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync(); //?? Czy taka forma ma sens, bo wygląda to na trochę bzdurny interfejs. Czy warto w ogóle dodać np. interface rozszerzający IBase o metodę GetAll?
        //?? W takim razie co powinno być zwracane z tasków zamiast IActionResult?
    }
}
