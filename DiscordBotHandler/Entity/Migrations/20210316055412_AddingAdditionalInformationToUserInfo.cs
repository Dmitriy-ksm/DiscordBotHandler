using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBotHandler.Entity.Migrations
{
    public partial class AddingAdditionalInformationToUserInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalInformationJSON",
                table: "UserInfos",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalInformationJSON",
                table: "UserInfos");
        }
    }
}
