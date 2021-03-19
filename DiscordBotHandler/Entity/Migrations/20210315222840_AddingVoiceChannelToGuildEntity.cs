using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBotHandler.Entity.Migrations
{
    public partial class AddingVoiceChannelToGuildEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Guids_GuildId",
                table: "Channels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Guids",
                table: "Guids");

            migrationBuilder.RenameTable(
                name: "Guids",
                newName: "Guilds");

            migrationBuilder.AddColumn<ulong>(
                name: "VoiceChannelId",
                table: "Guilds",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Guilds",
                table: "Guilds",
                column: "GuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Guilds_GuildId",
                table: "Channels",
                column: "GuildId",
                principalTable: "Guilds",
                principalColumn: "GuildId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Guilds_GuildId",
                table: "Channels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Guilds",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "VoiceChannelId",
                table: "Guilds");

            migrationBuilder.RenameTable(
                name: "Guilds",
                newName: "Guids");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Guids",
                table: "Guids",
                column: "GuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Guids_GuildId",
                table: "Channels",
                column: "GuildId",
                principalTable: "Guids",
                principalColumn: "GuildId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
