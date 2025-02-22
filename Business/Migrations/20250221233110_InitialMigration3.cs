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
            migrationBuilder.AddColumn<int>(
                name: "ParentFlowParameterId",
                table: "FlowParameter",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "FlowParameter",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_FlowParameter_ParentFlowParameterId",
                table: "FlowParameter",
                column: "ParentFlowParameterId");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowParameter_FlowParameter_ParentFlowParameterId",
                table: "FlowParameter",
                column: "ParentFlowParameterId",
                principalTable: "FlowParameter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlowParameter_FlowParameter_ParentFlowParameterId",
                table: "FlowParameter");

            migrationBuilder.DropIndex(
                name: "IX_FlowParameter_ParentFlowParameterId",
                table: "FlowParameter");

            migrationBuilder.DropColumn(
                name: "ParentFlowParameterId",
                table: "FlowParameter");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "FlowParameter");
        }
    }
}
