using RestApiTest.Core.Models;
using System.Threading.Tasks;

namespace RestApiTest.Core.Interfaces
{
    public interface IMarkable
    {
        Task AddMarkAsync(Vote voteToAdd); //[Note] - 'Task' to wskazówka dla używającego interfejs, że to może potencjalnie trwać dłużej - Czy w praktyce w web dev'ie nawet takie potencjalnie krótkie metody robi się asynchroniczne?
        Task RemoveMarkAsync(long id);
        long GetAllMarksOfGivenType(bool refersToPositive, long relatedItemId); //[Note] Jeśli jest logika biznesowa, to poza repo, to co może być selectem rozwiązane to w repo - Czy tego typu operacja nadaje się do interfejsu repo, czy raczej już powinna być w innej warstwie?
    }
}
