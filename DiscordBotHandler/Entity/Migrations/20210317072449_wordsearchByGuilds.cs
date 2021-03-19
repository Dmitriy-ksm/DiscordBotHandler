using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBotHandler.Entity.Migrations
{
    public partial class wordsearchByGuilds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "GuildId",
                table: "WordSearches",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuildId",
                table: "WordSearches");
        }
    }
}
