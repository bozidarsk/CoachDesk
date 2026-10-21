using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoachDesk.Migrations
{
    /// <inheritdoc />
    public partial class AddFKCoachIdToMembershipPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembershipPlans_Coaches_CoachId",
                table: "MembershipPlans");

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipPlans_Coaches_CoachId",
                table: "MembershipPlans",
                column: "CoachId",
                principalTable: "Coaches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembershipPlans_Coaches_CoachId",
                table: "MembershipPlans");

            migrationBuilder.AddForeignKey(
                name: "FK_MembershipPlans_Coaches_CoachId",
                table: "MembershipPlans",
                column: "CoachId",
                principalTable: "Coaches",
                principalColumn: "Id");
        }
    }
}
