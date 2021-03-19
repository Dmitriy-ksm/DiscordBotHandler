using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBotHandler.Entity.Migrations
{
    public partial class ChangeLogic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelsCommandAccess");

            migrationBuilder.AddColumn<string>(
                name: "CommandAccessCommand",
                table: "Channels",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Channels_CommandAccessCommand",
                table: "Channels",
                column: "CommandAccessCommand");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_CommandAccesses_CommandAccessCommand",
                table: "Channels",
                column: "CommandAccessCommand",
                principalTable: "CommandAccesses",
                principalColumn: "Command",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_CommandAccesses_CommandAccessCommand",
                table: "Channels");

            migrationBuilder.DropIndex(
                name: "IX_Channels_CommandAccessCommand",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "CommandAccessCommand",
                table: "Channels");

            migrationBuilder.CreateTable(
                name: "ChannelsCommandAccess",
                columns: table => new
                {
                    ChannelsChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    CommandsCommand = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelsCommandAccess", x => new { x.ChannelsChannelId, x.CommandsCommand });
                    table.ForeignKey(
                        name: "FK_ChannelsCommandAccess_Channels_ChannelsChannelId",
                        column: x => x.ChannelsChannelId,
                        principalTable: "Channels",
                        principalColumn: "ChannelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChannelsCommandAccess_CommandAccesses_CommandsCommand",
                        column: x => x.CommandsCommand,
                        principalTable: "CommandAccesses",
                        principalColumn: "Command",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelsCommandAccess_CommandsCommand",
                table: "ChannelsCommandAccess",
                column: "CommandsCommand");
        }
    }
}
