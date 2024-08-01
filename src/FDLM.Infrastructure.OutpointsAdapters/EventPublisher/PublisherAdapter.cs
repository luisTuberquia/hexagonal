using Amazon.EventBridge;
using Amazon.EventBridge.Model;
using FDLM.Application.Ports;
using FDLM.Domain.Models;
using FDLM.Domain.Models.Result;
using FDLM.Infrastructure.OutpointsAdapters.EventPublisher.Config;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FDLM.Infrastructure.OutpointsAdapters.EventPublisher
{
    internal class PublisherAdapter : IPublisherAdapterPort
    {
        private readonly IAmazonEventBridge _eventBridgeClient;
        private readonly IOptions<EventBridgeOptions> _eventBridgeOptions;

        public PublisherAdapter(IAmazonEventBridge eventBridgeClient, IOptions<EventBridgeOptions> eventBridgeOptions)
        {
            _eventBridgeClient = eventBridgeClient ?? throw new ArgumentNullException(nameof(eventBridgeClient));
            _eventBridgeOptions = eventBridgeOptions ?? throw new ArgumentNullException(nameof(eventBridgeOptions));
        }

        public async Task<Results<bool>> SendSumRequestAsync(List<string> addends)
        {
            var eventConfig = _eventBridgeOptions.Value.Events["SumRequest"];

            string detail = JsonConvert.SerializeObject(new { Addends = addends });
            var response = await SendEventAsync(eventConfig.EventBusName, eventConfig.DetailType, eventConfig.Source, detail);
            return response;
        }

        public async Task<Results<bool>> SendSumResultAsync(CalculatorOperation operation)
        {
            var eventConfig = _eventBridgeOptions.Value.Events["SumResult"];

            string detail = JsonConvert.SerializeObject(operation);
            var response = await SendEventAsync(eventConfig.EventBusName, eventConfig.DetailType, eventConfig.Source, detail);
            return response;
        }

        public async Task<Results<bool>> SendEventAsync(string eventBusName, string detailType, string source, string detail)
        {
            var results = new Results<bool>(true);

            var request = new PutEventsRequest
            {
                Entries = new List<PutEventsRequestEntry>
            {
                new PutEventsRequestEntry
                {
                    EventBusName = eventBusName,
                    Detail = detail,
                    DetailType = detailType,
                    Source = source,
                    Time = DateTime.UtcNow
                }
            }
            };

            var response = await _eventBridgeClient.PutEventsAsync(request);

            if (response.FailedEntryCount > 0)
            {
                results.Result = false;
                results.AddError("something went wrong sending the event");
            }
            return results;
        }

    }
}
