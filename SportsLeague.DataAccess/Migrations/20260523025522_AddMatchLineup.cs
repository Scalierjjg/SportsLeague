using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsLeague.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchLineup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchLineup_Players_PlayerId",
                table: "MatchLineup");

            migrationBuilder.DropIndex(
                name: "IX_MatchLineup_MatchId",
                table: "MatchLineup");

            migrationBuilder.AlterColumn<string>(
                name: "Position",
                table: "MatchLineup",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_MatchLineup_MatchId_PlayerId",
                table: "MatchLineup",
                columns: new[] { "MatchId", "PlayerId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchLineup_Players_PlayerId",
                table: "MatchLineup",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchLineup_Players_PlayerId",
                table: "MatchLineup");

            migrationBuilder.DropIndex(
                name: "IX_MatchLineup_MatchId_PlayerId",
                table: "MatchLineup");

            migrationBuilder.AlterColumn<string>(
                name: "Position",
                table: "MatchLineup",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.CreateIndex(
                name: "IX_MatchLineup_MatchId",
                table: "MatchLineup",
                column: "MatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchLineup_Players_PlayerId",
                table: "MatchLineup",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
