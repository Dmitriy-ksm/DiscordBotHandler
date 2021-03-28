using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBotHandler.Entity.Migrations
{
    public partial class AddingCommandCooldownEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cooldowns",
                columns: table => new
                {
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    KeyCooldown = table.Column<ulong>(type: "INTEGER", nullable: false),
                    LastUse = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cooldowns", x => x.Key);
                });

            migrationBuilder.InsertData(
                table: "Cooldowns",
                columns: new[] { "Key", "KeyCooldown", "LastUse" },
                values: new object[] { "wordsearch", 60ul, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cooldowns");

        }
    }
}
