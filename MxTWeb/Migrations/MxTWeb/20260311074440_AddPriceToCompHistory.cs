using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MxTWeb.Migrations.MxTWeb
{
    /// <inheritdoc />
    public partial class AddPriceToCompHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "CompHistory",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "isUsed",
                table: "ChipPressure",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "waferNumber",
                table: "ChipPressure",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "CompHistory");

            migrationBuilder.DropColumn(
                name: "isUsed",
                table: "ChipPressure");

            migrationBuilder.DropColumn(
                name: "waferNumber",
                table: "ChipPressure");
        }
    }
}
