using MassTransit;

using System;
using System.Threading.Tasks;

namespace Orchestrator.Consumers.Payments;

public class RefundPaymentConsumer : IConsumer<RefundPaymentCommand>
{
    public async Task Consume(ConsumeContext<RefundPaymentCommand> context)
    {
        var message = context.Message;

        // TODO: Implement refund logic
        await Task.Delay(500);

        Console.WriteLine($"[PaymentService] Refunding payment completed for Order {message.OrderId}");

        await Task.CompletedTask;
    }
}
