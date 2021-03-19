using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBotHandler.Entity.Migrations
{
    public partial class wordsearchByGuildsFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WordSearches",
                table: "WordSearches");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "WordSearches",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Reply",
                table: "WordSearches",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "WordSearches",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<ulong>(
                name: "WordSearchByGuildsGuildId",
                table: "WordSearches",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WordSearches",
                table: "WordSearches",
                column: "Id");

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

            migrationBuilder.CreateIndex(
                name: "IX_WordSearches_WordSearchByGuildsGuildId",
                table: "WordSearches",
                column: "WordSearchByGuildsGuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_WordSearches_WordSearchesByGuilds_WordSearchByGuildsGuildId",
                table: "WordSearches",
                column: "WordSearchByGuildsGuildId",
                principalTable: "WordSearchesByGuilds",
                principalColumn: "GuildId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordSearches_WordSearchesByGuilds_WordSearchByGuildsGuildId",
                table: "WordSearches");

            migrationBuilder.DropTable(
                name: "WordSearchesByGuilds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WordSearches",
                table: "WordSearches");

            migrationBuilder.DropIndex(
                name: "IX_WordSearches_WordSearchByGuildsGuildId",
                table: "WordSearches");

            migrationBuilder.DropColumn(
                name: "WordSearchByGuildsGuildId",
                table: "WordSearches");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "WordSearches",
                newName: "GuildId");

            migrationBuilder.AlterColumn<string>(
                name: "Reply",
                table: "WordSearches",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "GuildId",
                table: "WordSearches",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WordSearches",
                table: "WordSearches",
                column: "Reply");
        }
    }
}
