using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TicketOffice.Orchestrator.Persistence;

/// <summary>
/// Maps the PurchaseState entity to the database.
/// </summary>
public class PurchaseStateMap : SagaClassMap<PurchaseState>
{
    protected override void Configure(EntityTypeBuilder<PurchaseState> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.Property(x => x.Amount);
        entity.Property(x => x.SeatNumber);
        entity.Property(x => x.RowNumber);

        entity.Property(x => x.PaymentStepResult).HasConversion<string>().HasMaxLength(32);
        entity.Property(x => x.BookingStepResult).HasConversion<string>().HasMaxLength(32);
        entity.Property(x => x.TicketGenerationStepRsult).HasConversion<string>().HasMaxLength(32);
    }
}