using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketOffice.Orchestrator.Migrations
{
    /// <inheritdoc />
    public partial class InitSagaDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseState",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    RowNumber = table.Column<int>(type: "integer", nullable: false),
                    SeatNumber = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentStepResult = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    BookingStepResult = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    TicketGenerationStepRsult = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CurrentState = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseState", x => x.CorrelationId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseState");
        }
    }
}
