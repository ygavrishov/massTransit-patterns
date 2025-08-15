using Microsoft.AspNetCore.Mvc;

using TicketOffice.API.Infrastructure;
using TicketOffice.API.Models.Requests;
using TicketOffice.Contracts;

namespace TicketOffice.API.Controllers;

[ApiController]
[Route("api/v1/tickets")]
public class TicketController(IPurchaseTicketSender sender) : ControllerBase
{

    [HttpPost]
    [Route("purchase")]
    public async Task<IActionResult> PurchaseTicket(PurchaseTicketRequest request)
    {
        var command = new PurchaseTicketCommand
        (
            OrderId: Guid.NewGuid(),
            RowNumber: request.RowNumber,
            SeatNumber: request.SeatNumber
        );

        await sender.SendAsync(command);

        return Accepted();
    }
}
