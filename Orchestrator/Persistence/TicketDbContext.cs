using System.Collections.Generic;

using MassTransit.EntityFrameworkCoreIntegration;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Orchestrator.Persistence;

public class TicketDbContext(DbContextOptions options, IConfiguration configuration) : SagaDbContext(options)
{
    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new PurchaseStateMap(); }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(configuration["DefaultConnection"]);
        }
    }
}