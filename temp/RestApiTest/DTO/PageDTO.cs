using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiTest.DTO
{
    public class PageDTO<T>
    {
        public PageDTO(ICollection<T> items, int totalPages, string nextPageUrl)
        {
            this.Items = items;
            this.TotalPages = totalPages;
            this.NextPage = nextPageUrl;
        }
        public ICollection<T> Items { get; }
        public int TotalPages { get;}
        public string NextPage { get; }
    }
}
