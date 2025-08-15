using MassTransit;

using TicketOffice.DocumentService.Consumers;

namespace TicketOffice.DocumentService;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args);

        builder.ConfigureServices((context, services) =>
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<GenerateTicketConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint("document-service", e =>
                    {
                        e.ConfigureConsumer<GenerateTicketConsumer>(context);
                    });
                });
            });
        });
        await builder.Build().RunAsync();
    }
}
