namespace TicketOffice.Contracts;

// Commands
public record PurchaseTicketCommand(Guid OrderId, int RowNumber, int SeatNumber);
public record ReserveSeatCommand(Guid OrderId, int RowNumber, int SeatNumber);
public record ReleaseSeatCommand(Guid OrderId);
public record ProcessPaymentCommand(Guid OrderId, decimal Amount);
public record RefundPaymentCommand(Guid OrderId);
public record GenerateTicketCommand(Guid OrderId);

// Events
public record SeatReserved(Guid OrderId, int RowNumber, int SeatNumber);
public record SeatReservationFailed(Guid OrderId);

public record PaymentProcessed(Guid OrderId);
public record PaymentFailed(Guid OrderId);

public record TicketGenerated(Guid OrderId);
public record TicketGenerationFailed(Guid OrderId);
