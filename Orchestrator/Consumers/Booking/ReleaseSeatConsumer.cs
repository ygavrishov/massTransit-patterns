using MassTransit;

using System;
using System.Threading.Tasks;

namespace Orchestrator.Consumers.Booking;

public class ReleaseSeatConsumer : IConsumer<ReleaseSeatCommand>
{
    public async Task Consume(ConsumeContext<ReleaseSeatCommand> context)
    {
        var message = context.Message;

        Console.WriteLine($"[BookingService] seat was released for Order {message.OrderId}");

        // TODO: Implement logic to release the seat

        await Task.CompletedTask;
    }
}
