using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Business.Migrations
{
    /// <inheritdoc />
    public partial class MigrationFixDeleteExecution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Executions_Executions_ChildExecutionId",
                table: "Executions");

            migrationBuilder.AddForeignKey(
                name: "FK_Executions_Executions_ChildExecutionId",
                table: "Executions",
                column: "ChildExecutionId",
                principalTable: "Executions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Executions_Executions_ChildExecutionId",
                table: "Executions");

            migrationBuilder.AddForeignKey(
                name: "FK_Executions_Executions_ChildExecutionId",
                table: "Executions",
                column: "ChildExecutionId",
                principalTable: "Executions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
