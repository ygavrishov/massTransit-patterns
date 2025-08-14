using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Orchestrator.Persistence;

public class PurchaseStateMap : SagaClassMap<PurchaseState>
{
    protected override void Configure(EntityTypeBuilder<PurchaseState> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.Property(x => x.Amount);
        entity.Property(x => x.SeatNumber);
        entity.Property(x => x.RowNumber);
    }
}