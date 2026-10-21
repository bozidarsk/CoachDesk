using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoachDesk.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMembershipPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_Coaches_CoachId",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_CoachId",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "CoachId",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Memberships");

            migrationBuilder.RenameColumn(
                name: "Count",
                table: "Memberships",
                newName: "ClientId");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActivatedOn",
                table: "Memberships",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "MembershipPlanId",
                table: "Memberships",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimesUsed",
                table: "Memberships",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MembershipPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    CoachId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MembershipPlans_Coaches_CoachId",
                        column: x => x.CoachId,
                        principalTable: "Coaches",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_ClientId",
                table: "Memberships",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_MembershipPlanId",
                table: "Memberships",
                column: "MembershipPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipPlans_CoachId",
                table: "MembershipPlans",
                column: "CoachId");

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_Clients_ClientId",
                table: "Memberships",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_MembershipPlans_MembershipPlanId",
                table: "Memberships",
                column: "MembershipPlanId",
                principalTable: "MembershipPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_Clients_ClientId",
                table: "Memberships");

            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_MembershipPlans_MembershipPlanId",
                table: "Memberships");

            migrationBuilder.DropTable(
                name: "MembershipPlans");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_ClientId",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_MembershipPlanId",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "ActivatedOn",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "MembershipPlanId",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "TimesUsed",
                table: "Memberships");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "Memberships",
                newName: "Count");

            migrationBuilder.AddColumn<int>(
                name: "CoachId",
                table: "Memberships",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "Memberships",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Memberships",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Memberships",
                type: "decimal(19,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_CoachId",
                table: "Memberships",
                column: "CoachId");

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_Coaches_CoachId",
                table: "Memberships",
                column: "CoachId",
                principalTable: "Coaches",
                principalColumn: "Id");
        }
    }
}
