using Amazon.SQS;
using Amazon.SQS.Model;
using FDLM.Application.Ports;
using FDLM.Application.UseCases.Calculator;
using FDLM.Domain.Models;
using FDLM.Domain.Models.Result;
using FDLM.Infrastructure.EntrypointsAdapters.Resources;
using FDLM.Infrastructure.EntrypointsAdapters.SQS.Dtos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FDLM.Infrastructure.EntrypointsAdapters.SQS
{
    public class SqsListener : ISqsListenerPort
    {
        private readonly AmazonSQSClient _sqsClient;
        private readonly ICalculatorUseCase _useCase;
        private readonly ILogger<SqsListener> _logger;


        public SqsListener(ILogger<SqsListener> logger, ICalculatorUseCase useCase)
        {
            _logger = logger;
            _useCase = useCase;
            _sqsClient = new AmazonSQSClient();
        }

        public async Task<Results<bool>> ListenForSumRequestsAsync(string queueUrl, int maxNumberOfMessages, int waitTimeSeconds)
        {
            var response = new Results<bool>();

            List<SumRequestEvent> sumRequests = await ReceiveMessagesAsync<SumRequestEvent>(queueUrl, maxNumberOfMessages, waitTimeSeconds);
            
            response.TotalItemsReturned = maxNumberOfMessages;
            response.TotalItemsInDataBase = sumRequests.Count;

            foreach (var request in sumRequests)
            {
                var result = await SumAndSaveOperationAsync(request.Detail);
                if (result.IsSuccess)
                {
                    response.Result = true;

                }
                else
                {
                    response.AddErrors(result.Errors);
                }
            }
            return response;
        }

        private async Task<Results<bool>> SumAndSaveOperationAsync(SumRequest sumRequest)
        {
            var response = new Results<bool>();
            try
            {
                IList<Number> addends = sumRequest.Addends.Select(a => (Number)new IntegerNumber(int.Parse(a))).ToList();
                var result = await _useCase.SumAndSaveOperationAsync(addends);
                if (result.IsSuccess)
                {
                    response.Result = true;
                    response.TotalItemsReturned = 1;
                    response.TotalItemsInDataBase = 0;
                }
                else
                {
                    response.AddErrors(result.Errors);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return response;
        }

        private async Task<List<T>> ReceiveMessagesAsync<T>(string queueUrl, int maxNumberOfMessages, int waitTimeSeconds)
        {
            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = maxNumberOfMessages,
                WaitTimeSeconds = waitTimeSeconds
            };

            var response = await _sqsClient.ReceiveMessageAsync(receiveMessageRequest);
            var deserializedMessages = new List<T>();

            foreach (var message in response.Messages)
            {
                var body = message.Body;

                try
                {
                    var deserializedMessage = JsonConvert.DeserializeObject<T>(body);
                    deserializedMessages.Add(deserializedMessage);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Failed to deserialize message body to {typeof(T)}: {ex.Message}");
                    continue;
                }

                var deleteMessageRequest = new DeleteMessageRequest
                {
                    QueueUrl = queueUrl,
                    ReceiptHandle = message.ReceiptHandle
                };
                await _sqsClient.DeleteMessageAsync(deleteMessageRequest);
            }

            return deserializedMessages;
        }

    }
}
