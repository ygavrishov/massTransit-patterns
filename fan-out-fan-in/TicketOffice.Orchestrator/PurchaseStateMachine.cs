using MassTransit;

using System;
using System.Threading.Tasks;

namespace TicketOffice.Orchestrator;

/// <summary>
/// Saga state machine for managing purchase transactions.
/// </summary>
public class PurchaseStateMachine : MassTransitStateMachine<PurchaseState>
{
    public State ReservationRequested { get; private set; }
    public State InProgress { get; private set; }
    public State Failed { get; private set; }
    public State Completed { get; private set; }

    public Event<PurchaseTicketCommand> ReservationRequestedEvent { get; private set; }
    public Event<SeatReserved> SeatReservedEvent { get; private set; }
    public Event<SeatReservationFailed> SeatReservationFailedEvent { get; private set; }
    public Event<PaymentProcessed> PaymentProcessedEvent { get; private set; }
    public Event<PaymentFailed> PaymentFailedEvent { get; private set; }
    public Event<TicketGenerated> TicketGeneratedEvent { get; private set; }
    public Event<TicketGenerationFailed> TicketGenerationFailedEvent { get; private set; }

    public PurchaseStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => ReservationRequestedEvent, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => SeatReservedEvent, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => SeatReservationFailedEvent, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentProcessedEvent, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentFailedEvent, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => TicketGeneratedEvent, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => TicketGenerationFailedEvent, x => x.CorrelateById(context => context.Message.OrderId));

        Initially(
            When(ReservationRequestedEvent)
                .Then(ctx =>
                {
                    ctx.Saga.RowNumber = ctx.Message.RowNumber;
                    ctx.Saga.SeatNumber = ctx.Message.SeatNumber;
                    ctx.Saga.Amount = Random.Shared.Next(100, 500); // TODO: Replace with actual amount logic
                })
                .Send(new Uri("queue:booking-service"), ctx =>
                    new ReserveSeatCommand(ctx.Saga.CorrelationId, ctx.Message.RowNumber, ctx.Message.SeatNumber))
                .Send(new Uri("queue:payment-service"), ctx =>
                    new ProcessPaymentCommand(ctx.Saga.CorrelationId, ctx.Saga.Amount))
                .Send(new Uri("queue:document-service"), ctx =>
                    new GenerateTicketCommand(ctx.Saga.CorrelationId))
                .TransitionTo(ReservationRequested)
        );

        During(ReservationRequested,
            When(SeatReservedEvent)
                .Then(ctx => ctx.Saga.BookingStepResult = StepResult.Success)
                .ThenAsync(CheckIfAllCompletedOrFailed),
            When(PaymentProcessedEvent)
                .Then(ctx => ctx.Saga.PaymentStepResult = StepResult.Success)
                .ThenAsync(CheckIfAllCompletedOrFailed),
            When(TicketGeneratedEvent)
                .Then(ctx => ctx.Saga.TicketGenerationStepRsult = StepResult.Success)
                .ThenAsync(CheckIfAllCompletedOrFailed),
            When(SeatReservationFailedEvent)
                .Then(ctx => ctx.Saga.BookingStepResult = StepResult.Failed)
                .ThenAsync(CheckIfAllCompletedOrFailed),
            When(PaymentFailedEvent)
                .Then(ctx => ctx.Saga.PaymentStepResult = StepResult.Failed)
                .ThenAsync(CheckIfAllCompletedOrFailed),
            When(TicketGenerationFailedEvent)
                .Then(ctx => ctx.Saga.TicketGenerationStepRsult = StepResult.Failed)
                .ThenAsync(CheckIfAllCompletedOrFailed)
        );
    }

    private async Task CheckIfAllCompletedOrFailed(BehaviorContext<PurchaseState> context)
    {
        var saga = context.Saga;
        Console.WriteLine($"State of order {saga.CorrelationId}: " +
                          $"Payment: {saga.PaymentStepResult}, " +
                          $"Booking: {saga.BookingStepResult}, " +
                          $"Ticket Generation: {saga.TicketGenerationStepRsult}");
        if (saga.AllStepsAreCompleted)
        {
            if (saga.AllStepsAreSuccessful)
            {
                saga.CurrentState = Completed.Name;
                Console.WriteLine($"Order {saga.CorrelationId} was processed successfully.");
            }
            else
            {
                // At least one step failed
                await PerformCompensations(context);
                saga.CurrentState = Failed.Name;
                Console.WriteLine($"Order {saga.CorrelationId} processing failed.");
            }
        }
    }

    private async Task PerformCompensations(BehaviorContext<PurchaseState> context)
    {
        var saga = context.Saga;

        // send compensation commands for each successful step
        if (saga.PaymentStepResult == StepResult.Success)
            await context.Send(new Uri("queue:payment-service"), new RefundPaymentCommand(saga.CorrelationId));

        if (saga.BookingStepResult == StepResult.Success)
            await context.Send(new Uri("queue:booking-service"), new ReleaseSeatCommand(saga.CorrelationId));
    }
}
