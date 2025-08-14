using MassTransit;

using System;
using System.Threading.Tasks;

namespace Orchestrator.Consumers.Documents;

public class GenerateTicketConsumer : IConsumer<GenerateTicketCommand>
{
    public async Task Consume(ConsumeContext<GenerateTicketCommand> context)
    {
        var message = context.Message;

        bool generated = await GenerateTicketFile(message.OrderId);

        if (generated)
        {
            Console.WriteLine($"[DocumentService] Ticket was generated for Order {message.OrderId}");
            await context.Publish(new TicketGenerated(message.OrderId));
        }
        else
        {
            Console.WriteLine($"[DocumentService] Ticket generation failed for Order {message.OrderId}");
            await context.Publish(new TicketGenerationFailed(message.OrderId));
        }
    }

    private async Task<bool> GenerateTicketFile(Guid orderId)
    {
        // TODO: create PDF, write to storage
        await Task.Delay(500);
        return true;
    }
}
