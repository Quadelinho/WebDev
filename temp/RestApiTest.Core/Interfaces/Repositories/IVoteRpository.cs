using RestApiTest.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestApiTest.Core.Interfaces.Repositories
{
    public interface IVoteRpository : IBaseRepository<Vote>
    {
        //?? Czy w praktyce stosuje się zwracanie całego aktualizowanego obiektu? Jeśli tak, czy nie jest to security issue (wysyłając request'a aktualizacji ktoś może dostać w zwrotce komplet informacji o stanie obiektu / choć z drugiej strony mając uprawnienia do update'u ma też pewnie do get'a).
        //[Note] - da się to skonfigurować prawdopodobnie w middlewere'ze, jako zachowanie w przypadku niezdefiniowanych pól (np. czy ma je zastępować pustymi, czy nie robić z nimi nic) - Czy z automatu framework ogarnia rozróżnienie między niepełnym obiektem, a wartościami null'owymi (np. że jeśli nie podam w ogóle pola w body request'u, to pole nie zostanie zaktualizowane na bazie, a jeśli chcę wyzerować dane pole, to mogę w body podać pole = null), czy wszystko to trzeba ręcznie obsłużyć?
        Task<ForumUser> GetVoterDetails(long voteId);
        Task<IVotable> GetVotedSubject(long voteId);
        Task<IEnumerable<Vote>> GetAllVotesForObject(IMarkable relatedObject);
    }
}
