using FDLM.Application.Ports;
using FDLM.Infrastructure.EntrypointsAdapters.SQS;
using FDLM.Infrastructure.EntrypointsAdapters.SQS.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace FDLM.Infrastructure.EntrypointsAdapters.Resources
{
    internal class SqsSumListenerJob : IJob
    {
        private readonly SqsListener _sqsEntryPoint;
        private readonly string _queueUrl;
        private readonly int _maxNumberOfMessages;
        private readonly int _waitTimeSeconds;
        private readonly ILogger<SqsSumListenerJob> _logger;


        public SqsSumListenerJob(SqsListener sqsEntryPoint,ILogger<SqsSumListenerJob> logger, IOptions<SumRequestConfig> options)
        {
            _logger = logger;
            _sqsEntryPoint = sqsEntryPoint;
            _queueUrl = options.Value.QueueUrl ??
                throw new ArgumentException("QueueUrl was not found in configuration");
            _maxNumberOfMessages = options.Value.MaxNumberOfMessages ?? 2;
            _waitTimeSeconds = options.Value.WaitTimeSeconds ?? 30;
        }

        public async Task Execute(IJobExecutionContext context)
        {
           var result =  await _sqsEntryPoint.ListenForSumRequestsAsync(_queueUrl, _maxNumberOfMessages, _waitTimeSeconds);
            if (result.IsSuccess)
            {
                _logger.LogInformation("success");
            }
            else
            {
                _logger.LogError(result.Errors.ToString());
            }
        }
    }
}
