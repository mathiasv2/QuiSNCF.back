using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuiSNCF.Migrations
{
    /// <inheritdoc />
    public partial class indexex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DailyPlays_PlayerId",
                table: "DailyPlays");

            migrationBuilder.CreateIndex(
                name: "IX_Players_Name",
                table: "Players",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_DailyPlays_PlayerId_GameType_PlayedDate",
                table: "DailyPlays",
                columns: new[] { "PlayerId", "GameType", "PlayedDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Players_Name",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_DailyPlays_PlayerId_GameType_PlayedDate",
                table: "DailyPlays");

            migrationBuilder.CreateIndex(
                name: "IX_DailyPlays_PlayerId",
                table: "DailyPlays",
                column: "PlayerId");
        }
    }
}
