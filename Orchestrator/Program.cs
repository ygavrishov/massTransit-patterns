using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Orchestrator.Consumers.Booking;
using Orchestrator.Consumers.Documents;
using Orchestrator.Consumers.Payments;
using Orchestrator.Persistence;

using System.Reflection;
using System.Threading.Tasks;

namespace Orchestrator;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = CreateHostBuilder(args).Build();
        await app.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<TicketPurchaseStarter>();
                //services.AddDbContext<TicketDbContext>(options =>
                //{
                //    options.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection"));
                //});
                services.AddMassTransit(x =>
                {
                    x.AddDelayedMessageScheduler();

                    x.SetKebabCaseEndpointNameFormatter();

                    //x.AddEntityFrameworkSagaRepository<PurchaseState, SagaDbContext>();
                    x.AddSagaStateMachine<PurchaseStateMachine, PurchaseState>()
                    .EntityFrameworkRepository(r =>
                     {
                         r.ConcurrencyMode = ConcurrencyMode.Optimistic;

                         r.AddDbContext<DbContext, TicketDbContext>((provider, builder) =>
                         {
                             builder.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection"));
                         });
                     });

                    var entryAssembly = Assembly.GetEntryAssembly();

                    x.AddConsumers(entryAssembly);
                    //x.AddSagaStateMachines(entryAssembly);
                    //x.AddSagas(entryAssembly);
                    x.AddActivities(entryAssembly);

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host("localhost", "/", h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });
                        cfg.UseDelayedMessageScheduler();

                        cfg.ReceiveEndpoint("ticket-purchase-saga", e =>
                        {
                            e.ConfigureSaga<PurchaseState>(context);
                        });

                        cfg.ReceiveEndpoint("booking-service", e =>
                        {
                            e.ConfigureConsumer<ReserveSeatConsumer>(context);
                            e.ConfigureConsumer<ReleaseSeatConsumer>(context);
                        });

                        cfg.ReceiveEndpoint("payment-service", e =>
                        {
                            e.ConfigureConsumer<ProcessPaymentConsumer>(context);
                            e.ConfigureConsumer<RefundPaymentConsumer>(context);
                        });

                        cfg.ReceiveEndpoint("document-service", e =>
                        {
                            e.ConfigureConsumer<GenerateTicketConsumer>(context);
                        });
                    });

                });
            });
}
