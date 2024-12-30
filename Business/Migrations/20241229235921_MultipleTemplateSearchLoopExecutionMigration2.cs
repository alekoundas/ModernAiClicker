using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Business.Migrations
{
    /// <inheritdoc />
    public partial class MultipleTemplateSearchLoopExecutionMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreviousLoopResultImagePath",
                table: "Executions");

            migrationBuilder.RenameColumn(
                name: "PreviousLoopResultImagePath",
                table: "FlowSteps",
                newName: "LoopResultImagePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LoopResultImagePath",
                table: "FlowSteps",
                newName: "PreviousLoopResultImagePath");

            migrationBuilder.AddColumn<string>(
                name: "PreviousLoopResultImagePath",
                table: "Executions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
