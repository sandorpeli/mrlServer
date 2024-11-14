using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MRLserver.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MRLmodel",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    telepitesHelye = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    telepitestVegezte = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    telepitesPozicioja = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    karbantarto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    telepitesIdeje = table.Column<DateTime>(type: "datetime2", nullable: true),
                    utolsoKarbantartasIdeje = table.Column<DateTime>(type: "datetime2", nullable: true),
                    kovetkezoKarbantartas = table.Column<DateTime>(type: "datetime2", nullable: true),
                    utolsoKapcsolataLifttel = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MRLmodel", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MRLmodel");
        }
    }
}
