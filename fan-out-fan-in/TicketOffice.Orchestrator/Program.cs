using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Reflection;
using System.Threading.Tasks;

using TicketOffice.Orchestrator.Persistence;

namespace TicketOffice.Orchestrator;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = CreateHostBuilder(args).Build();
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<TicketDbContext>();
            db.Database.Migrate();
        }
        await app.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext<TicketDbContext>(options =>
                {
                    options.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection"));
                });
                services.AddMassTransit(x =>
                {
                    x.AddDelayedMessageScheduler();

                    x.SetKebabCaseEndpointNameFormatter();

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
                    x.AddActivities(entryAssembly);

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host("rabbitmq", "/", h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });
                        cfg.UseDelayedMessageScheduler();

                        cfg.ReceiveEndpoint("ticket-purchase-saga", e =>
                        {
                            e.ConfigureSaga<PurchaseState>(context);
                        });
                    });

                });
            });
}
