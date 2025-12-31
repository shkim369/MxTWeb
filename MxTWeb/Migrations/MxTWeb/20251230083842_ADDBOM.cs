using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MxTWeb.Migrations.MxTWeb
{
    /// <inheritdoc />
    public partial class ADDBOM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompHistory_Component_ComponentId",
                table: "CompHistory");

            migrationBuilder.DropColumn(
                name: "PartNumber",
                table: "CompHistory");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Component",
                newName: "ComponentId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CompHistory",
                newName: "CompHistoryId");

            migrationBuilder.AlterColumn<int>(
                name: "ComponentId",
                table: "CompHistory",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Bom",
                columns: table => new
                {
                    BomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BomVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bom", x => x.BomId);
                });

            migrationBuilder.CreateTable(
                name: "BomDetail",
                columns: table => new
                {
                    BomDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BomId = table.Column<int>(type: "int", nullable: false),
                    ComponentId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BomDetail", x => x.BomDetailId);
                    table.ForeignKey(
                        name: "FK_BomDetail_Bom_BomId",
                        column: x => x.BomId,
                        principalTable: "Bom",
                        principalColumn: "BomId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BomDetail_Component_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Component",
                        principalColumn: "ComponentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BomDetail_BomId",
                table: "BomDetail",
                column: "BomId");

            migrationBuilder.CreateIndex(
                name: "IX_BomDetail_ComponentId",
                table: "BomDetail",
                column: "ComponentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompHistory_Component_ComponentId",
                table: "CompHistory",
                column: "ComponentId",
                principalTable: "Component",
                principalColumn: "ComponentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompHistory_Component_ComponentId",
                table: "CompHistory");

            migrationBuilder.DropTable(
                name: "BomDetail");

            migrationBuilder.DropTable(
                name: "Bom");

            migrationBuilder.RenameColumn(
                name: "ComponentId",
                table: "Component",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CompHistoryId",
                table: "CompHistory",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "ComponentId",
                table: "CompHistory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "PartNumber",
                table: "CompHistory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_CompHistory_Component_ComponentId",
                table: "CompHistory",
                column: "ComponentId",
                principalTable: "Component",
                principalColumn: "Id");
        }
    }
}
