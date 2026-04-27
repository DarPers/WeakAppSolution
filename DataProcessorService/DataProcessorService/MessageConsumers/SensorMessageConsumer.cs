using DataIngestorService.Contracts.Contracts;
using DataProcessorService.DAL.Interfaces;
using DataProcessorService.DAL.Entities;
using MassTransit;
using SensorEvent = DataProcessorService.DAL.Entities.SensorEvent;

namespace DataProcessorService.MessageConsumers;

public class SensorMessageConsumer(ISensorEventRepository sensorEventRepository) : IConsumer<SensorEventsMessage>
{
    public async Task Consume(ConsumeContext<SensorEventsMessage> context)
    {
        Console.WriteLine("ooo, che to prishlo!!!"); //TODO BLL ^)

        var message = context.Message;

        var sensorEvents = context.Message.SensorEvents.Select(e => new SensorEvent
        {
            Id = Guid.NewGuid(),
            Type = e.Type,
            Name = e.Name,
            Payload = e.Payload,
            ReceivedAt = message.ReceivedAt,
        }).ToList();

        await sensorEventRepository.Create(sensorEvents);
    }

    public Task ReceiveSensorData()
    {
        throw new NotImplementedException();
    }
}
