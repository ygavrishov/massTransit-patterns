using MassTransit;

using System;

namespace TicketOffice.Orchestrator;

/// <summary>
/// Saga state machine for managing purchase transactions.
/// </summary>
public class PurchaseStateMachine : MassTransitStateMachine<PurchaseState>
{
    public State ReservationRequested { get; private set; }
    public State Reserved { get; private set; }
    public State Paid { get; private set; }
    public State TicketGenerated { get; private set; }
    public State Failed { get; private set; }

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
                .TransitionTo(ReservationRequested)
        );

        During(ReservationRequested,
            When(SeatReservedEvent)
                .Send(new Uri("queue:payment-service"), ctx =>
                    new ProcessPaymentCommand(ctx.Saga.CorrelationId, ctx.Saga.Amount))
                .TransitionTo(Reserved)
        );

        During(ReservationRequested,
            When(SeatReservationFailedEvent)
                .Then(ctx =>
                {
                    Console.WriteLine($"Seat reservation failed for Order {ctx.Message.OrderId}");
                })
                .TransitionTo(Failed)
                .Finalize()
        );

        During(Reserved,
            When(PaymentProcessedEvent)
                .TransitionTo(Paid)
                .Send(new Uri("queue:document-service"), ctx =>
                    new GenerateTicketCommand(ctx.Saga.CorrelationId))
        );

        During(Reserved,
            When(PaymentFailedEvent)
                .Send(new Uri("queue:booking-service"), ctx =>
                    new ReleaseSeatCommand(ctx.Saga.CorrelationId))
                .TransitionTo(Failed)
                .Finalize()
        );

        During(Paid,
            When(TicketGeneratedEvent)
                .TransitionTo(TicketGenerated)
                .Then(ctx => Console.WriteLine($"Order {ctx.Message.OrderId} was processed successfully."))
                .Finalize()
        );

        During(Paid,
            When(TicketGenerationFailedEvent)
                .Send(new Uri("queue:payment-service"), ctx =>
                    new RefundPaymentCommand(ctx.Saga.CorrelationId))
                .Then(ctx => Console.WriteLine($"Order {ctx.Message.OrderId} processing failed."))
                .TransitionTo(Failed)
                .Finalize()
        );
    }
}
