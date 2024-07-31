using FDLM.Domain.Models.Result;

namespace FDLM.Application.Ports
{
    public interface ISqsListenerPort
    {
        Task<Results<bool>> ListenForSumRequestsAsync(string queueUrl, int maxNumberOfMessages, int waitTimeSeconds);
    }
}
