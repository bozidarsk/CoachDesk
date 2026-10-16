using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoachDesk.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterTablesForIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_UserCredentials_UserCredentialsId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Coaches_UserCredentials_UserCredentialsId",
                table: "Coaches");

            migrationBuilder.DropTable(
                name: "UserCredentials");

            migrationBuilder.DropIndex(
                name: "IX_Coaches_UserCredentialsId",
                table: "Coaches");

            migrationBuilder.DropIndex(
                name: "IX_Clients_UserCredentialsId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "UserCredentialsId",
                table: "Coaches");

            migrationBuilder.DropColumn(
                name: "UserCredentialsId",
                table: "Clients");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Coaches",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Clients",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Coaches_UserId",
                table: "Coaches",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_UserId",
                table: "Clients",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_AspNetUsers_UserId",
                table: "Clients",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Coaches_AspNetUsers_UserId",
                table: "Coaches",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_AspNetUsers_UserId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Coaches_AspNetUsers_UserId",
                table: "Coaches");

            migrationBuilder.DropIndex(
                name: "IX_Coaches_UserId",
                table: "Coaches");

            migrationBuilder.DropIndex(
                name: "IX_Clients_UserId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Coaches");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Clients");

            migrationBuilder.AddColumn<int>(
                name: "UserCredentialsId",
                table: "Coaches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserCredentialsId",
                table: "Clients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserCredentials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCredentials", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coaches_UserCredentialsId",
                table: "Coaches",
                column: "UserCredentialsId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_UserCredentialsId",
                table: "Clients",
                column: "UserCredentialsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_UserCredentials_UserCredentialsId",
                table: "Clients",
                column: "UserCredentialsId",
                principalTable: "UserCredentials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Coaches_UserCredentials_UserCredentialsId",
                table: "Coaches",
                column: "UserCredentialsId",
                principalTable: "UserCredentials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
