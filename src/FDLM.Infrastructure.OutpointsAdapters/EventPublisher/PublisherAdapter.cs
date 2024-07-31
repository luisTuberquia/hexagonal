using Amazon.EventBridge;
using Amazon.EventBridge.Model;
using FDLM.Application.Ports;

namespace FDLM.Infrastructure.OutpointsAdapters.EventPublisher
{
    internal class PublisherAdapter : IPublisherAdapterPort
    {
        private readonly IAmazonEventBridge _eventBridgeClient;


        public PublisherAdapter(IAmazonEventBridge eventBridgeClient)
        {
            _eventBridgeClient = eventBridgeClient ?? throw new ArgumentNullException(nameof(eventBridgeClient));
        }

        public async Task SendEventAsync(string eventBusName, string detailType, string source, string detail)
        {
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
                throw new Exception($"Failed to send event: {response.Entries[0].ErrorMessage}");
            }
        }

    }
}
