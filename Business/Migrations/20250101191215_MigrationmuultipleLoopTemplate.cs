using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Business.Migrations
{
    /// <inheritdoc />
    public partial class MigrationmuultipleLoopTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlowSteps_FlowSteps_ParentTemplateSearchFlowStepId",
                table: "FlowSteps");

            migrationBuilder.AddColumn<int>(
                name: "CurrentMultipleTemplateSearchFlowStepId",
                table: "Executions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Executions_CurrentMultipleTemplateSearchFlowStepId",
                table: "Executions",
                column: "CurrentMultipleTemplateSearchFlowStepId");

            migrationBuilder.AddForeignKey(
                name: "FK_Executions_FlowSteps_CurrentMultipleTemplateSearchFlowStepId",
                table: "Executions",
                column: "CurrentMultipleTemplateSearchFlowStepId",
                principalTable: "FlowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowSteps_FlowSteps_ParentTemplateSearchFlowStepId",
                table: "FlowSteps",
                column: "ParentTemplateSearchFlowStepId",
                principalTable: "FlowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Executions_FlowSteps_CurrentMultipleTemplateSearchFlowStepId",
                table: "Executions");

            migrationBuilder.DropForeignKey(
                name: "FK_FlowSteps_FlowSteps_ParentTemplateSearchFlowStepId",
                table: "FlowSteps");

            migrationBuilder.DropIndex(
                name: "IX_Executions_CurrentMultipleTemplateSearchFlowStepId",
                table: "Executions");

            migrationBuilder.DropColumn(
                name: "CurrentMultipleTemplateSearchFlowStepId",
                table: "Executions");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowSteps_FlowSteps_ParentTemplateSearchFlowStepId",
                table: "FlowSteps",
                column: "ParentTemplateSearchFlowStepId",
                principalTable: "FlowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
