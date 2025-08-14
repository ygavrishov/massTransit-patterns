using MassTransit;

using Microsoft.Extensions.Hosting;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Orchestrator;

public class TicketPurchaseStarter(IBus bus) : BackgroundService
{

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("[Starter] Sending SeatReservationRequest event...");

        await bus.Publish(new PurchaseTicketCommand(
            OrderId: Guid.NewGuid(),
            RowNumber: 5,
            SeatNumber: 12
        ), stoppingToken);
        Console.WriteLine("[Starter] SeatReservationRequest event sent");
    }
}
