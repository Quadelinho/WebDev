using System.Threading.Tasks;

namespace RestApiTest.Core.Interfaces
{
    public interface IMarkable
    {
        Task AddMarkAsync(bool refersToPositive); //[Note] - 'Task' to wskazówka dla używającego interfejs, że to może potencjalnie trwać dłużej - Czy w praktyce w web dev'ie nawet takie potencjalnie krótkie metody robi się asynchroniczne?
        Task RemoveMarkAsync(bool refersToPositive);
        Task<long> GetAllMarksOfGivenType(bool refersToPositive); //[Note] Jeśli jest logika biznesowa, to poza repo, to co może być selectem rozwiązane to w repo - Czy tego typu operacja nadaje się do interfejsu repo, czy raczej już powinna być w innej warstwie?
    }
}
