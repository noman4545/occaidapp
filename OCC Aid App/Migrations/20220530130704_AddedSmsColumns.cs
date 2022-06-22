using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OCC_Aid_App.Migrations
{
    public partial class AddedSmsColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDmReviewed",
                table: "ArchievedSMSs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequiredDmReview",
                table: "ArchievedSMSs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDmReviewed",
                table: "ArchievedSMSs");

            migrationBuilder.DropColumn(
                name: "IsRequiredDmReview",
                table: "ArchievedSMSs");
        }
    }
}
