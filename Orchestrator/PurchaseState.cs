using System;

namespace Orchestrator;

using MassTransit;

public class PurchaseState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }

    public int RowNumber { get; set; }
    public int SeatNumber { get; set; }
    public decimal Amount { get; set; }

    public string CurrentState { get; set; }
}
