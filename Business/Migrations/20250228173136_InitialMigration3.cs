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
            migrationBuilder.DropForeignKey(
                name: "FK_FlowSteps_Flows_SubFlowId",
                table: "FlowSteps");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowSteps_Flows_SubFlowId",
                table: "FlowSteps",
                column: "SubFlowId",
                principalTable: "Flows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlowSteps_Flows_SubFlowId",
                table: "FlowSteps");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowSteps_Flows_SubFlowId",
                table: "FlowSteps",
                column: "SubFlowId",
                principalTable: "Flows",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
