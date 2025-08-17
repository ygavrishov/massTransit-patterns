using MassTransit;

using TicketOffice.Contracts;

namespace TicketOffice.BookingService.Consumers;

public class ReserveSeatConsumer : IConsumer<ReserveSeatCommand>
{
    public async Task Consume(ConsumeContext<ReserveSeatCommand> context)
    {
        var message = context.Message;

        // TODO: Implement logic to check seat availability
        await Task.Delay(300);
        bool seatAvailable = true;

        if (seatAvailable)
        {
            Console.WriteLine($"[BookingService] Seat {message.RowNumber}-{message.SeatNumber} was reserved for Order {message.OrderId}");
            await context.Publish(new SeatReserved(message.OrderId, message.RowNumber, message.SeatNumber));
        }
        else
        {
            Console.WriteLine($"[BookingService] Seat reservation failed for Order {message.OrderId}");
            await context.Publish(new SeatReservationFailed(message.OrderId));
        }
    }
}
