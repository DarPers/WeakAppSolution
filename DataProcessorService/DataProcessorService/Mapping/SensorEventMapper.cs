using DataIngestorService.Contracts.Contracts;
using SensorEventEntity = DataProcessorService.DAL.Entities.SensorEvent;

namespace DataProcessorService.Mapping;

public static class SensorEventMapper
{
    public static List<SensorEventEntity> ToEntities(SensorEventsMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(message.SensorEvents);

        return message.SensorEvents
            .Select(e => new SensorEventEntity
            {
                Id = Guid.NewGuid(),
                Type = e.Type,
                Name = e.Name,
                Payload = e.Payload,
                ReceivedAt = message.ReceivedAt,
            })
            .ToList();
    }
}
