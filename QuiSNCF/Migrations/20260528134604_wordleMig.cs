using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace QuiSNCF.Migrations
{
    /// <inheritdoc />
    public partial class wordleMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScoreDate",
                table: "Players");

            migrationBuilder.CreateTable(
                name: "DailyPlays",
                columns: table => new
                {
                    DailyPlayId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    GameType = table.Column<int>(type: "integer", nullable: false),
                    PlayedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Tries = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyPlays", x => x.DailyPlayId);
                    table.ForeignKey(
                        name: "FK_DailyPlays_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Words",
                columns: table => new
                {
                    WordId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WordName = table.Column<string>(type: "text", nullable: false),
                    Definition = table.Column<string>(type: "text", nullable: false),
                    LastTimePlayed = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Words", x => x.WordId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyPlays_PlayerId",
                table: "DailyPlays",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyPlays");

            migrationBuilder.DropTable(
                name: "Words");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ScoreDate",
                table: "Players",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }
    }
}
