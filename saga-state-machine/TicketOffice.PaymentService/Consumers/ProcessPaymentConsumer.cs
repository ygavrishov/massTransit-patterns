using MassTransit;

using TicketOffice.Contracts;

namespace TicketOffice.PaymentService.Consumers;

public class ProcessPaymentConsumer : IConsumer<ProcessPaymentCommand>
{
    public async Task Consume(ConsumeContext<ProcessPaymentCommand> context)
    {
        var message = context.Message;

        // TODO: Implement payment processing logic
        await Task.Delay(500);
        bool paymentSuccessful = true;

        if (paymentSuccessful)
        {
            Console.WriteLine($"[PaymentService] payment {message.Amount} was processed for Order {message.OrderId}");
            await context.Publish(new PaymentProcessed(message.OrderId));
        }
        else
        {
            Console.WriteLine($"[PaymentService] payment {message.Amount} processing failed for Order {message.OrderId}");
            await context.Publish(new PaymentFailed(message.OrderId));
        }
    }
}
