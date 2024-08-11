using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Business.Migrations
{
    /// <inheritdoc />
    public partial class Migration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlowSteps_FlowSteps_ParentFlowStepId",
                table: "FlowSteps");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowSteps_FlowSteps_ParentFlowStepId",
                table: "FlowSteps",
                column: "ParentFlowStepId",
                principalTable: "FlowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlowSteps_FlowSteps_ParentFlowStepId",
                table: "FlowSteps");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowSteps_FlowSteps_ParentFlowStepId",
                table: "FlowSteps",
                column: "ParentFlowStepId",
                principalTable: "FlowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
