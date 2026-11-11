using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MxTWeb.Migrations.MxTWeb
{
    /// <inheritdoc />
    public partial class AddQueryIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "state",
                table: "CompHistory",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "WaferInfo",
                table: "ChipUsage",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Lot",
                table: "ChipUsage",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Division",
                table: "ChipUsage",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Chip",
                table: "ChipUsage",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "WaferInfo",
                table: "ChipPressure",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Lot",
                table: "ChipPressure",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_CompHistory_EntryDate",
                table: "CompHistory",
                column: "EntryDate");

            migrationBuilder.CreateIndex(
                name: "IX_CompHistory_state_Date",
                table: "CompHistory",
                columns: new[] { "state", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_ChipUsage_Chip_UseDate",
                table: "ChipUsage",
                columns: new[] { "Chip", "UseDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ChipUsage_Division_UseDate",
                table: "ChipUsage",
                columns: new[] { "Division", "UseDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ChipPressure_Lot",
                table: "ChipPressure",
                column: "Lot");

            migrationBuilder.CreateIndex(
                name: "IX_ChipPressure_TestDate",
                table: "ChipPressure",
                column: "TestDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CompHistory_EntryDate",
                table: "CompHistory");

            migrationBuilder.DropIndex(
                name: "IX_CompHistory_state_Date",
                table: "CompHistory");

            migrationBuilder.DropIndex(
                name: "IX_ChipUsage_Chip_UseDate",
                table: "ChipUsage");

            migrationBuilder.DropIndex(
                name: "IX_ChipUsage_Division_UseDate",
                table: "ChipUsage");

            migrationBuilder.DropIndex(
                name: "IX_ChipPressure_Lot",
                table: "ChipPressure");

            migrationBuilder.DropIndex(
                name: "IX_ChipPressure_TestDate",
                table: "ChipPressure");

            migrationBuilder.AlterColumn<string>(
                name: "state",
                table: "CompHistory",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "WaferInfo",
                table: "ChipUsage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Lot",
                table: "ChipUsage",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Division",
                table: "ChipUsage",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Chip",
                table: "ChipUsage",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "WaferInfo",
                table: "ChipPressure",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Lot",
                table: "ChipPressure",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
