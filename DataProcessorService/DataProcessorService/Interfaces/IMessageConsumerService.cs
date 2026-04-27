namespace DataProcessorService.Interfaces;

public interface IMessageConsumerService
{
    Task ReceiveSensorData();
}
