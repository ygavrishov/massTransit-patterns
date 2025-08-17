using System;

namespace TicketOffice.Orchestrator;

using MassTransit;

public enum StepResult { Pending, Success, Failed }

/// <summary>
/// Saga state for managing purchase transactions.
/// </summary>
public class PurchaseState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }

    public int RowNumber { get; set; }
    public int SeatNumber { get; set; }
    public decimal Amount { get; set; }

    public StepResult PaymentStepResult { get; set; }
    public StepResult BookingStepResult { get; set; }
    public StepResult TicketGenerationStepRsult { get; set; }

    public bool AllStepsAreCompleted => 
        PaymentStepResult != StepResult.Pending &&
        BookingStepResult != StepResult.Pending &&
        TicketGenerationStepRsult != StepResult.Pending;

    public bool AllStepsAreSuccessful => 
        PaymentStepResult == StepResult.Success &&
        BookingStepResult == StepResult.Success &&
        TicketGenerationStepRsult == StepResult.Success;

    public string CurrentState { get; set; }
}
