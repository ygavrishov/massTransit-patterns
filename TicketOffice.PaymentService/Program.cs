using MassTransit;

using TicketOffice.PaymentService.Consumers;

namespace TicketOffice.PaymentService;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args);

        builder.ConfigureServices((context, services) =>
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<ProcessPaymentConsumer>();
                x.AddConsumer<RefundPaymentConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint("payment-service", e =>
                    {
                        e.ConfigureConsumer<ProcessPaymentConsumer>(context);
                        e.ConfigureConsumer<RefundPaymentConsumer>(context);
                    });
                });
            });
        });
        await builder.Build().RunAsync();
    }
}
