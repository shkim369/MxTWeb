using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MxTWeb.Migrations.MxTWeb
{
    /// <inheritdoc />
    public partial class ModifyEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Manager",
                table: "Equipment",
                newName: "Manager_Sub");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PurchaseDate",
                table: "Equipment",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<string>(
                name: "Manager_Main",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Manager_Main",
                table: "Equipment");

            migrationBuilder.RenameColumn(
                name: "Manager_Sub",
                table: "Equipment",
                newName: "Manager");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "PurchaseDate",
                table: "Equipment",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
