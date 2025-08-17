## Purpose

The solution enhances the [basic saga implementation](../saga-state-machine/README.md) with fan-in, fan-out pattern. Hence the downward services are informed **at the same time** and work **in parallel**. The saga moves to the next and final step when it receives events from all invoked services. If any of them fails then the compensation is requested for those of services that processed successfully.

Here is the sequence diagram for the saga:

```mermaid
sequenceDiagram
    participant Client
    participant Orchestrator
    participant BookingService
    participant PaymentService
    participant TicketService

    Client->>Orchestrator: Purchase Ticket Request

    par Parallel Steps
        Orchestrator->>BookingService: Reserve Seat
        Orchestrator->>PaymentService: Process Payment
        Orchestrator->>TicketService: Generate Ticket
    end

    alt All Success
        BookingService-->>Orchestrator: Seat Reserved
        PaymentService-->>Orchestrator: Payment Processed
        TicketService-->>Orchestrator: Ticket Generated
        Orchestrator-->>Client: Ticket & Confirmation
    else Any Failure
        alt Booking Failed
            BookingService-->>Orchestrator: Error
        end
        alt Payment Failed
            PaymentService-->>Orchestrator: Error
        end
        alt Ticket Generation Failed
            TicketService-->>Orchestrator: Error
        end

        Note right of Orchestrator: Send compensaction command to the services that processed successfully

        alt Booking Success
            Orchestrator->>BookingService: Release Seat (compensation)
        end
        alt Payment Success
            Orchestrator->>PaymentService: Initiate Refund (compensation)
        end
        alt Ticket Generation Success
            Orchestrator->>TicketService: Cancel Ticket (compensation)
        end

        Orchestrator-->>Client: Purchase Failed
    end
```

## Technologies

The solution uses RabbitMQ as a transport, saga state is stored in PostgreSQL. All services are written in .NET 9 with MassTransit as a out-of-the-box implementation of Saga Pattern.

## How to run

* Run docker compose from the current folder with the command: 
```
docker compose up  -d
```
* Open http://localhost:8080/swagger in browser. Invoke `/api/v1/tickets/purchase` method with some `rowNumber` and `seatNumber`.

* Look into Postgres database for the saga state:
```
docker compose exec postgres psql -U root -d TicketSaga
```
Run the query:
```
select * from "PurchaseState";
```
You are expected to see a row that is corresponded with the initiated request and the state should be `CurrentState=Completed`.
