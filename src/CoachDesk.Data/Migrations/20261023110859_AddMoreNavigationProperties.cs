using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoachDesk.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreNavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_Clients_ClientId",
                table: "Memberships");

            migrationBuilder.AlterColumn<Guid>(
                name: "ClientId",
                table: "Memberships",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_Clients_ClientId",
                table: "Memberships",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_Clients_ClientId",
                table: "Memberships");

            migrationBuilder.AlterColumn<Guid>(
                name: "ClientId",
                table: "Memberships",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_Clients_ClientId",
                table: "Memberships",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");
        }
    }
}
