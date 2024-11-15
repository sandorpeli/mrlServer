using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MRLserver.Migrations
{
    /// <inheritdoc />
    public partial class AddTelemetryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MRLtelemetryModel",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    utolsoKapcsolataLifttel = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DoorStateA = table.Column<int>(type: "int", nullable: false),
                    DoorStateB = table.Column<int>(type: "int", nullable: false),
                    ElevatorState = table.Column<int>(type: "int", nullable: false),
                    Travel1 = table.Column<int>(type: "int", nullable: false),
                    Travel2 = table.Column<int>(type: "int", nullable: false),
                    VVVFErrors = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Errors = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MRLtelemetryModel", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MRLtelemetryModel");
        }
    }
}
