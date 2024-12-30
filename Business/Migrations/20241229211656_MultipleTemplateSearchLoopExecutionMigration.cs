using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Business.Migrations
{
    /// <inheritdoc />
    public partial class MultipleTemplateSearchLoopExecutionMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentLoopCount",
                table: "Executions",
                newName: "ParentLoopExecutionId");

            migrationBuilder.AddColumn<int>(
                name: "ChildLoopExecutionId",
                table: "Executions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoopCount",
                table: "Executions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviousLoopResultImagePath",
                table: "Executions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Executions_ChildLoopExecutionId",
                table: "Executions",
                column: "ChildLoopExecutionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Executions_Executions_ChildLoopExecutionId",
                table: "Executions",
                column: "ChildLoopExecutionId",
                principalTable: "Executions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Executions_Executions_ChildLoopExecutionId",
                table: "Executions");

            migrationBuilder.DropIndex(
                name: "IX_Executions_ChildLoopExecutionId",
                table: "Executions");

            migrationBuilder.DropColumn(
                name: "ChildLoopExecutionId",
                table: "Executions");

            migrationBuilder.DropColumn(
                name: "LoopCount",
                table: "Executions");

            migrationBuilder.DropColumn(
                name: "PreviousLoopResultImagePath",
                table: "Executions");

            migrationBuilder.RenameColumn(
                name: "ParentLoopExecutionId",
                table: "Executions",
                newName: "CurrentLoopCount");
        }
    }
}
