using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Business.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlowParameter_Flows_FlowId",
                table: "FlowParameter");

            migrationBuilder.DropForeignKey(
                name: "FK_FlowSteps_Flows_FlowId",
                table: "FlowSteps");

            migrationBuilder.AddColumn<int>(
                name: "FlowParameterId",
                table: "Flows",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FlowStepId",
                table: "Flows",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Flows_FlowParameterId",
                table: "Flows",
                column: "FlowParameterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Flows_FlowStepId",
                table: "Flows",
                column: "FlowStepId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowParameter_Flows_FlowId",
                table: "FlowParameter",
                column: "FlowId",
                principalTable: "Flows",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Flows_FlowParameter_FlowParameterId",
                table: "Flows",
                column: "FlowParameterId",
                principalTable: "FlowParameter",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Flows_FlowSteps_FlowStepId",
                table: "Flows",
                column: "FlowStepId",
                principalTable: "FlowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowSteps_Flows_FlowId",
                table: "FlowSteps",
                column: "FlowId",
                principalTable: "Flows",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlowParameter_Flows_FlowId",
                table: "FlowParameter");

            migrationBuilder.DropForeignKey(
                name: "FK_Flows_FlowParameter_FlowParameterId",
                table: "Flows");

            migrationBuilder.DropForeignKey(
                name: "FK_Flows_FlowSteps_FlowStepId",
                table: "Flows");

            migrationBuilder.DropForeignKey(
                name: "FK_FlowSteps_Flows_FlowId",
                table: "FlowSteps");

            migrationBuilder.DropIndex(
                name: "IX_Flows_FlowParameterId",
                table: "Flows");

            migrationBuilder.DropIndex(
                name: "IX_Flows_FlowStepId",
                table: "Flows");

            migrationBuilder.DropColumn(
                name: "FlowParameterId",
                table: "Flows");

            migrationBuilder.DropColumn(
                name: "FlowStepId",
                table: "Flows");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowParameter_Flows_FlowId",
                table: "FlowParameter",
                column: "FlowId",
                principalTable: "Flows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowSteps_Flows_FlowId",
                table: "FlowSteps",
                column: "FlowId",
                principalTable: "Flows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
