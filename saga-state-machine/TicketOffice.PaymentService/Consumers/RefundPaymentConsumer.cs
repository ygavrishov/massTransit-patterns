using MassTransit;

using TicketOffice.Contracts;

namespace TicketOffice.PaymentService.Consumers;

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
