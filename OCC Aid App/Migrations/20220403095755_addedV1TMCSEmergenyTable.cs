using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OCC_Aid_App.Migrations
{
    public partial class addedV1TMCSEmergenyTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "V1_TMCSEmergencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    BlockId = table.Column<int>(type: "int", nullable: true),
                    DmDecision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EfcMarkedCompleted = table.Column<bool>(type: "bit", nullable: false),
                    Completed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_V1_TMCSEmergencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_V1_TMCSEmergencies_V1_Blocks_BlockId",
                        column: x => x.BlockId,
                        principalTable: "V1_Blocks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_V1_TMCSEmergencies_V1_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "V1_Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_V1_TMCSEmergencies_BlockId",
                table: "V1_TMCSEmergencies",
                column: "BlockId");

            migrationBuilder.CreateIndex(
                name: "IX_V1_TMCSEmergencies_ZoneId",
                table: "V1_TMCSEmergencies",
                column: "ZoneId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "V1_TMCSEmergencies");
        }
    }
}
