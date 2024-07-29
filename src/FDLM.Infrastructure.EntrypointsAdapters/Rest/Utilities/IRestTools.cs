using FDLM.Domain.Models.Result;

namespace FDLM.Infrastructure.EntrypointsAdapters.Rest.Utilities
{
    public interface IRestTools
    {
        int GetHttpStatusCode(List<FdlmError> errors);
    }
}