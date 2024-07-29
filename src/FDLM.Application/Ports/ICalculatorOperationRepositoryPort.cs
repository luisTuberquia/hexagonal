using FDLM.Domain.Models;
using FDLM.Domain.Models.Result;

namespace FDLM.Application.Ports
{
    public interface ICalculatorOperationRepositoryPort
    {
        Task<Results<CalculatorOperation>> SaveAsync(CalculatorOperation model);
        Task<Results<CalculatorOperation>> FindByIdAsync(string id);
        Task<Results<IList<CalculatorOperation>>> FindByCreationDateAsync(DateTime startDateUtc, DateTime endDateUtc, int pageNumber, int pageSize, string sortField, SortDirection sortDirection);
        Task<Results<long>> Count(DateTime startDateUtc, DateTime endDateUtc);
    }
}
