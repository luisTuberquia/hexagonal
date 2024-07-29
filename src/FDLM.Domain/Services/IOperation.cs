using FDLM.Domain.Models;
using FDLM.Domain.Models.Result;

namespace FDLM.Domain.Services
{
    public interface IOperation
    {
        Task<Results<Number>> SumAsync(IList<Number> addends);
    }
}