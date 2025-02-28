using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Business.Migrations
{
    /// <inheritdoc />
    public partial class Flow_Add_ParentSubFlowStep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flows_FlowSteps_ParentFlowStepId",
                table: "Flows");

            migrationBuilder.RenameColumn(
                name: "ParentFlowStepId",
                table: "Flows",
                newName: "ParentSubFlowStepId");

            migrationBuilder.RenameIndex(
                name: "IX_Flows_ParentFlowStepId",
                table: "Flows",
                newName: "IX_Flows_ParentSubFlowStepId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flows_FlowSteps_ParentSubFlowStepId",
                table: "Flows",
                column: "ParentSubFlowStepId",
                principalTable: "FlowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flows_FlowSteps_ParentSubFlowStepId",
                table: "Flows");

            migrationBuilder.RenameColumn(
                name: "ParentSubFlowStepId",
                table: "Flows",
                newName: "ParentFlowStepId");

            migrationBuilder.RenameIndex(
                name: "IX_Flows_ParentSubFlowStepId",
                table: "Flows",
                newName: "IX_Flows_ParentFlowStepId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flows_FlowSteps_ParentFlowStepId",
                table: "Flows",
                column: "ParentFlowStepId",
                principalTable: "FlowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
