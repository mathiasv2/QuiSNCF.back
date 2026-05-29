using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuiSNCF.Migrations
{
    /// <inheritdoc />
    public partial class testMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "PlayerId", "Name", "Score", "Tries" },
                values: new object[] { 1, "Caca", 100, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "PlayerId",
                keyValue: 1);
        }
    }
}
