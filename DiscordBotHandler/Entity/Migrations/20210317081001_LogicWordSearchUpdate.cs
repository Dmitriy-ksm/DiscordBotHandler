using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBotHandler.Entity.Migrations
{
    public partial class LogicWordSearchUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordSearches_WordSearchesByGuilds_WordSearchByGuildsGuildId",
                table: "WordSearches");

            migrationBuilder.DropTable(
                name: "WordSearchesByGuilds");

            migrationBuilder.RenameColumn(
                name: "WordSearchByGuildsGuildId",
                table: "WordSearches",
                newName: "GuildsGuildId");

            migrationBuilder.RenameIndex(
                name: "IX_WordSearches_WordSearchByGuildsGuildId",
                table: "WordSearches",
                newName: "IX_WordSearches_GuildsGuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_WordSearches_Guilds_GuildsGuildId",
                table: "WordSearches",
                column: "GuildsGuildId",
                principalTable: "Guilds",
                principalColumn: "GuildId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordSearches_Guilds_GuildsGuildId",
                table: "WordSearches");

            migrationBuilder.RenameColumn(
                name: "GuildsGuildId",
                table: "WordSearches",
                newName: "WordSearchByGuildsGuildId");

            migrationBuilder.RenameIndex(
                name: "IX_WordSearches_GuildsGuildId",
                table: "WordSearches",
                newName: "IX_WordSearches_WordSearchByGuildsGuildId");

            migrationBuilder.CreateTable(
                name: "WordSearchesByGuilds",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordSearchesByGuilds", x => x.GuildId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_WordSearches_WordSearchesByGuilds_WordSearchByGuildsGuildId",
                table: "WordSearches",
                column: "WordSearchByGuildsGuildId",
                principalTable: "WordSearchesByGuilds",
                principalColumn: "GuildId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
