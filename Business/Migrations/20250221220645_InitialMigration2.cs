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
                name: "FK_FlowSteps_FlowSteps_SearchAreaParameterFlowStepId",
                table: "FlowSteps");

            migrationBuilder.RenameColumn(
                name: "SearchAreaParameterFlowStepId",
                table: "FlowSteps",
                newName: "FlowParameterId");

            migrationBuilder.RenameIndex(
                name: "IX_FlowSteps_SearchAreaParameterFlowStepId",
                table: "FlowSteps",
                newName: "IX_FlowSteps_FlowParameterId");

            migrationBuilder.CreateTable(
                name: "FlowParameter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FlowId = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsExpanded = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSelected = table.Column<bool>(type: "INTEGER", nullable: false),
                    OrderingNum = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationX = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationY = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowParameter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlowParameter_Flows_FlowId",
                        column: x => x.FlowId,
                        principalTable: "Flows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlowParameter_FlowId",
                table: "FlowParameter",
                column: "FlowId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowParameter_Id",
                table: "FlowParameter",
                column: "Id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowSteps_FlowParameter_FlowParameterId",
                table: "FlowSteps",
                column: "FlowParameterId",
                principalTable: "FlowParameter",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlowSteps_FlowParameter_FlowParameterId",
                table: "FlowSteps");

            migrationBuilder.DropTable(
                name: "FlowParameter");

            migrationBuilder.RenameColumn(
                name: "FlowParameterId",
                table: "FlowSteps",
                newName: "SearchAreaParameterFlowStepId");

            migrationBuilder.RenameIndex(
                name: "IX_FlowSteps_FlowParameterId",
                table: "FlowSteps",
                newName: "IX_FlowSteps_SearchAreaParameterFlowStepId");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowSteps_FlowSteps_SearchAreaParameterFlowStepId",
                table: "FlowSteps",
                column: "SearchAreaParameterFlowStepId",
                principalTable: "FlowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
