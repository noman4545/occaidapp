using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OCC_Aid_App.Migrations
{
    public partial class addedV1Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "V1_Blocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_V1_Blocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "V1_Zones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FanDirection = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LeftName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RightName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShaftName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CctvLayout = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZoneLayout = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_V1_Zones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "V1_ZoneBlocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    BlockId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_V1_ZoneBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_V1_ZoneBlocks_V1_Blocks_BlockId",
                        column: x => x.BlockId,
                        principalTable: "V1_Blocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_V1_ZoneBlocks_V1_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "V1_Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_V1_ZoneBlocks_BlockId",
                table: "V1_ZoneBlocks",
                column: "BlockId");

            migrationBuilder.CreateIndex(
                name: "IX_V1_ZoneBlocks_ZoneId",
                table: "V1_ZoneBlocks",
                column: "ZoneId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "V1_ZoneBlocks");

            migrationBuilder.DropTable(
                name: "V1_Blocks");

            migrationBuilder.DropTable(
                name: "V1_Zones");
        }
    }
}
