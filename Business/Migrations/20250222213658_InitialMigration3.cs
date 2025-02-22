using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Business.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LocationY",
                table: "FlowParameters",
                newName: "LocationTop");

            migrationBuilder.RenameColumn(
                name: "LocationX",
                table: "FlowParameters",
                newName: "LocationRight");

            migrationBuilder.AddColumn<int>(
                name: "LocationBottom",
                table: "FlowParameters",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LocationLeft",
                table: "FlowParameters",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProcessName",
                table: "FlowParameters",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SystemMonitorDeviceName",
                table: "FlowParameters",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TemplateSearchAreaTypesEnum",
                table: "FlowParameters",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationBottom",
                table: "FlowParameters");

            migrationBuilder.DropColumn(
                name: "LocationLeft",
                table: "FlowParameters");

            migrationBuilder.DropColumn(
                name: "ProcessName",
                table: "FlowParameters");

            migrationBuilder.DropColumn(
                name: "SystemMonitorDeviceName",
                table: "FlowParameters");

            migrationBuilder.DropColumn(
                name: "TemplateSearchAreaTypesEnum",
                table: "FlowParameters");

            migrationBuilder.RenameColumn(
                name: "LocationTop",
                table: "FlowParameters",
                newName: "LocationY");

            migrationBuilder.RenameColumn(
                name: "LocationRight",
                table: "FlowParameters",
                newName: "LocationX");
        }
    }
}
