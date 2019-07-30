using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiTest.DTO
{
    public class PageDTO<T>
    {
        public ICollection<T> Items { get; }
        public int TotalPages { get;}
        public string NextPage { get; }
    }
}
