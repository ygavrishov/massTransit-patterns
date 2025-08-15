using MassTransit;

using TicketOffice.BookingService.Consumers;

namespace TicketOffice.BookingService;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args);

        builder.ConfigureServices((context, services) =>
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<ReserveSeatConsumer>();
                x.AddConsumer<ReleaseSeatConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint("booking-service", e =>
                    {
                        e.ConfigureConsumer<ReserveSeatConsumer>(context);
                        e.ConfigureConsumer<ReleaseSeatConsumer>(context);
                    });
                });
            });
        });
        await builder.Build().RunAsync();
    }
}
