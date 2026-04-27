using DataIngestorService.Contracts.Contracts;

namespace DataIngestorService.MessagingService.Interfaces;

public interface IMessageProducerService
{
    public Task PublishSensorData(SensorEventsMessage sensorEventMessage, CancellationToken cancellationToken);
}
