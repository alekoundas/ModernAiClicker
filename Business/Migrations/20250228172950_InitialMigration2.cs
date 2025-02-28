using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Business.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flows_FlowSteps_ParentSubFlowStepId",
                table: "Flows");

            migrationBuilder.AddForeignKey(
                name: "FK_Flows_FlowSteps_ParentSubFlowStepId",
                table: "Flows",
                column: "ParentSubFlowStepId",
                principalTable: "FlowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flows_FlowSteps_ParentSubFlowStepId",
                table: "Flows");

            migrationBuilder.AddForeignKey(
                name: "FK_Flows_FlowSteps_ParentSubFlowStepId",
                table: "Flows",
                column: "ParentSubFlowStepId",
                principalTable: "FlowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
