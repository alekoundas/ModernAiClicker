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
                name: "FK_FlowParameter_FlowParameter_ParentFlowParameterId",
                table: "FlowParameter");

            migrationBuilder.DropForeignKey(
                name: "FK_FlowParameter_Flows_FlowId",
                table: "FlowParameter");

            migrationBuilder.DropForeignKey(
                name: "FK_FlowSteps_FlowParameter_FlowParameterId",
                table: "FlowSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_FlowSteps_FlowSteps_ParentFlowStepId",
                table: "FlowSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FlowParameter",
                table: "FlowParameter");

            migrationBuilder.RenameTable(
                name: "FlowParameter",
                newName: "FlowParameters");

            migrationBuilder.RenameIndex(
                name: "IX_FlowParameter_ParentFlowParameterId",
                table: "FlowParameters",
                newName: "IX_FlowParameters_ParentFlowParameterId");

            migrationBuilder.RenameIndex(
                name: "IX_FlowParameter_Id",
                table: "FlowParameters",
                newName: "IX_FlowParameters_Id");

            migrationBuilder.RenameIndex(
                name: "IX_FlowParameter_FlowId",
                table: "FlowParameters",
                newName: "IX_FlowParameters_FlowId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FlowParameters",
                table: "FlowParameters",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowParameters_FlowParameters_ParentFlowParameterId",
                table: "FlowParameters",
                column: "ParentFlowParameterId",
                principalTable: "FlowParameters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowParameters_Flows_FlowId",
                table: "FlowParameters",
                column: "FlowId",
                principalTable: "Flows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowSteps_FlowParameters_FlowParameterId",
                table: "FlowSteps",
                column: "FlowParameterId",
                principalTable: "FlowParameters",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

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
                name: "FK_FlowParameters_FlowParameters_ParentFlowParameterId",
                table: "FlowParameters");

            migrationBuilder.DropForeignKey(
                name: "FK_FlowParameters_Flows_FlowId",
                table: "FlowParameters");

            migrationBuilder.DropForeignKey(
                name: "FK_FlowSteps_FlowParameters_FlowParameterId",
                table: "FlowSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_FlowSteps_FlowSteps_ParentFlowStepId",
                table: "FlowSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FlowParameters",
                table: "FlowParameters");

            migrationBuilder.RenameTable(
                name: "FlowParameters",
                newName: "FlowParameter");

            migrationBuilder.RenameIndex(
                name: "IX_FlowParameters_ParentFlowParameterId",
                table: "FlowParameter",
                newName: "IX_FlowParameter_ParentFlowParameterId");

            migrationBuilder.RenameIndex(
                name: "IX_FlowParameters_Id",
                table: "FlowParameter",
                newName: "IX_FlowParameter_Id");

            migrationBuilder.RenameIndex(
                name: "IX_FlowParameters_FlowId",
                table: "FlowParameter",
                newName: "IX_FlowParameter_FlowId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FlowParameter",
                table: "FlowParameter",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowParameter_FlowParameter_ParentFlowParameterId",
                table: "FlowParameter",
                column: "ParentFlowParameterId",
                principalTable: "FlowParameter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowParameter_Flows_FlowId",
                table: "FlowParameter",
                column: "FlowId",
                principalTable: "Flows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowSteps_FlowParameter_FlowParameterId",
                table: "FlowSteps",
                column: "FlowParameterId",
                principalTable: "FlowParameter",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowSteps_FlowSteps_ParentFlowStepId",
                table: "FlowSteps",
                column: "ParentFlowStepId",
                principalTable: "FlowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
