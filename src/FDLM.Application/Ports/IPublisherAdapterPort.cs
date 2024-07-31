namespace FDLM.Application.Ports
{
    public interface IPublisherAdapterPort
    {
        Task SendEventAsync(string eventBusName, string detailType, string source, string detail);
    }
}
