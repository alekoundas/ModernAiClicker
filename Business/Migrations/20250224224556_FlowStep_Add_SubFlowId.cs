using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Business.Migrations
{
    /// <inheritdoc />
    public partial class FlowStep_Add_SubFlowId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubFlowId",
                table: "FlowSteps",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlowSteps_SubFlowId",
                table: "FlowSteps",
                column: "SubFlowId");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowSteps_Flows_SubFlowId",
                table: "FlowSteps",
                column: "SubFlowId",
                principalTable: "Flows",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlowSteps_Flows_SubFlowId",
                table: "FlowSteps");

            migrationBuilder.DropIndex(
                name: "IX_FlowSteps_SubFlowId",
                table: "FlowSteps");

            migrationBuilder.DropColumn(
                name: "SubFlowId",
                table: "FlowSteps");
        }
    }
}
