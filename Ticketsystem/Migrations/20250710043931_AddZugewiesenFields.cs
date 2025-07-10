using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ticketsystem.Migrations
{
    /// <inheritdoc />
    public partial class AddZugewiesenFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_DeveloperId",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "DeveloperId",
                table: "Tickets",
                newName: "ZugewiesenerId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_DeveloperId",
                table: "Tickets",
                newName: "IX_Tickets_ZugewiesenerId");

            migrationBuilder.AddColumn<DateTime>(
                name: "ZugewiesenAm",
                table: "Tickets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_ZugewiesenerId",
                table: "Tickets",
                column: "ZugewiesenerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_ZugewiesenerId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ZugewiesenAm",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "ZugewiesenerId",
                table: "Tickets",
                newName: "DeveloperId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_ZugewiesenerId",
                table: "Tickets",
                newName: "IX_Tickets_DeveloperId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_DeveloperId",
                table: "Tickets",
                column: "DeveloperId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
