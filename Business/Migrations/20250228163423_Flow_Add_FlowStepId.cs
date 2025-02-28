using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Business.Migrations
{
    /// <inheritdoc />
    public partial class Flow_Add_FlowStepId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentFlowStepId",
                table: "Flows",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Flows_ParentFlowStepId",
                table: "Flows",
                column: "ParentFlowStepId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flows_FlowSteps_ParentFlowStepId",
                table: "Flows",
                column: "ParentFlowStepId",
                principalTable: "FlowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flows_FlowSteps_ParentFlowStepId",
                table: "Flows");

            migrationBuilder.DropIndex(
                name: "IX_Flows_ParentFlowStepId",
                table: "Flows");

            migrationBuilder.DropColumn(
                name: "ParentFlowStepId",
                table: "Flows");
        }
    }
}
