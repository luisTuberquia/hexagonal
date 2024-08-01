using FDLM.Domain.Models;
using FDLM.Domain.Models.Result;

namespace FDLM.Application.Ports
{
    public interface IPublisherAdapterPort
    {
        Task<Results<bool>> SendSumResultAsync(CalculatorOperation operation);
        Task<Results<bool>> SendSumRequestAsync(List<string> addends);
        Task<Results<bool>> SendEventAsync(string eventBusName, string detailType, string source, string detail);

    }
}
