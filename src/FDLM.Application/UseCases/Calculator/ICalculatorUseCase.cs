using FDLM.Domain.Models;
using FDLM.Domain.Models.Result;

namespace FDLM.Application.UseCases.Calculator
{
    public interface ICalculatorUseCase
    {
        Task<Results<Number>> SumAsync(IList<Number> addends);
        Task<Results<CalculatorOperation>> SaveOperationAsync(CalculatorOperation calculatorOperation);
        Task<Results<CalculatorOperation>> SumAndSaveOperationAsync(IList<Number> addends);
        Task<Results<IList<CalculatorOperation>>> GetOperationsByCreationDateAsync(DateTime startDateUtc, DateTime endDateUtc, int pageNumber, int pageSize, string sortField, SortDirection sortDirection);
        Task<Results<CalculatorOperation>> GetOperationAsync(string id);
    }
}