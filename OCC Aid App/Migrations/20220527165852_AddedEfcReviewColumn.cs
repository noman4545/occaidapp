using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OCC_Aid_App.Migrations
{
    public partial class AddedEfcReviewColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEfcRequireDmReview",
                table: "V1_TMCSEmergencies",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEfcRequireDmReview",
                table: "V1_TMCSEmergencies");
        }
    }
}
