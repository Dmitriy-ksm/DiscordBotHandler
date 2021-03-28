using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBotHandler.Entity.Migrations
{
    public partial class InitialDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommandAccesses",
                columns: table => new
                {
                    Command = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandAccesses", x => x.Command);
                });

            migrationBuilder.CreateTable(
                name: "CryptoInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EthUsd = table.Column<double>(type: "REAL", nullable: false),
                    GasAvarage = table.Column<int>(type: "INTEGER", nullable: false),
                    EthUsdTime = table.Column<string>(type: "TEXT", nullable: true),
                    EthBtc = table.Column<double>(type: "REAL", nullable: false),
                    EthBtcTime = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Guids",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guids", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "UserInfos",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SteamId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    ChannelId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.ChannelId);
                    table.ForeignKey(
                        name: "FK_Channels_Guids_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guids",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_Channels_GuildId",
                table: "Channels",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelsCommandAccess_CommandsCommand",
                table: "ChannelsCommandAccess",
                column: "CommandsCommand");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelsCommandAccess");

            migrationBuilder.DropTable(
                name: "CryptoInfo");

            migrationBuilder.DropTable(
                name: "UserInfos");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropTable(
                name: "CommandAccesses");

            migrationBuilder.DropTable(
                name: "Guids");
        }
    }
}
