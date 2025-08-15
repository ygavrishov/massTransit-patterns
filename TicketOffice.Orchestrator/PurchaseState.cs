using System;

namespace TicketOffice.Orchestrator;

using MassTransit;

/// <summary>
/// Saga state for managing purchase transactions.
/// </summary>
public class PurchaseState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }

    public int RowNumber { get; set; }
    public int SeatNumber { get; set; }
    public decimal Amount { get; set; }

    public string CurrentState { get; set; }
}
