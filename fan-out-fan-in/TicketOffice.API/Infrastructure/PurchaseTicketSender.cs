using MassTransit;

using TicketOffice.API.Configuration;
using TicketOffice.Contracts;

namespace TicketOffice.API.Infrastructure;

public interface IPurchaseTicketSender
{
    Task SendAsync(PurchaseTicketCommand command);
}

public class PurchaseTicketSender : IPurchaseTicketSender
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public PurchaseTicketSender(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task SendAsync(PurchaseTicketCommand command)
    {
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(
            new Uri($"queue:{QueueNames.PurchaseTicketSaga}"));

        await endpoint.Send(command);
    }
}