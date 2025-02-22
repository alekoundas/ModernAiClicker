using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Business.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Flows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FlowStepId = table.Column<int>(type: "INTEGER", nullable: false),
                    FlowParameterId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    IsSelected = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsExpanded = table.Column<bool>(type: "INTEGER", nullable: false),
                    OrderingNum = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlowParameter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FlowId = table.Column<int>(type: "INTEGER", nullable: true),
                    ParentFlowParameterId = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsExpanded = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSelected = table.Column<bool>(type: "INTEGER", nullable: false),
                    OrderingNum = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    LocationX = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationY = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowParameter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlowParameter_FlowParameter_ParentFlowParameterId",
                        column: x => x.ParentFlowParameterId,
                        principalTable: "FlowParameter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlowParameter_Flows_FlowId",
                        column: x => x.FlowId,
                        principalTable: "Flows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlowSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FlowId = table.Column<int>(type: "INTEGER", nullable: true),
                    ParentFlowStepId = table.Column<int>(type: "INTEGER", nullable: true),
                    FlowParameterId = table.Column<int>(type: "INTEGER", nullable: true),
                    ParentTemplateSearchFlowStepId = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ProcessName = table.Column<string>(type: "TEXT", nullable: false),
                    IsExpanded = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSelected = table.Column<bool>(type: "INTEGER", nullable: false),
                    OrderingNum = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    TemplateImage = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Accuracy = table.Column<decimal>(type: "TEXT", nullable: false),
                    LocationX = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationY = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxLoopCount = table.Column<int>(type: "INTEGER", nullable: false),
                    RemoveTemplateFromResult = table.Column<bool>(type: "INTEGER", nullable: false),
                    MouseAction = table.Column<int>(type: "INTEGER", nullable: false),
                    MouseButton = table.Column<int>(type: "INTEGER", nullable: false),
                    MouseScrollDirectionEnum = table.Column<int>(type: "INTEGER", nullable: false),
                    MouseLoopInfinite = table.Column<bool>(type: "INTEGER", nullable: false),
                    MouseLoopTimes = table.Column<int>(type: "INTEGER", nullable: false),
                    MouseLoopDebounceTime = table.Column<int>(type: "INTEGER", nullable: false),
                    MouseLoopTime = table.Column<TimeOnly>(type: "TEXT", nullable: true),
                    WaitForHours = table.Column<int>(type: "INTEGER", nullable: false),
                    WaitForMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    WaitForSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    WaitForMilliseconds = table.Column<int>(type: "INTEGER", nullable: false),
                    Height = table.Column<int>(type: "INTEGER", nullable: false),
                    Width = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlowSteps_FlowParameter_FlowParameterId",
                        column: x => x.FlowParameterId,
                        principalTable: "FlowParameter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FlowSteps_FlowSteps_ParentFlowStepId",
                        column: x => x.ParentFlowStepId,
                        principalTable: "FlowSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FlowSteps_FlowSteps_ParentTemplateSearchFlowStepId",
                        column: x => x.ParentTemplateSearchFlowStepId,
                        principalTable: "FlowSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FlowSteps_Flows_FlowId",
                        column: x => x.FlowId,
                        principalTable: "Flows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Executions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FlowId = table.Column<int>(type: "INTEGER", nullable: true),
                    FlowStepId = table.Column<int>(type: "INTEGER", nullable: true),
                    ParentExecutionId = table.Column<int>(type: "INTEGER", nullable: true),
                    ChildExecutionId = table.Column<int>(type: "INTEGER", nullable: true),
                    ParentLoopExecutionId = table.Column<int>(type: "INTEGER", nullable: true),
                    ChildLoopExecutionId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsExecuted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ExecutionResultEnum = table.Column<int>(type: "INTEGER", nullable: false),
                    RunFor = table.Column<string>(type: "TEXT", nullable: false),
                    StartedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsSelected = table.Column<bool>(type: "INTEGER", nullable: false),
                    LoopCount = table.Column<int>(type: "INTEGER", nullable: true),
                    ResultLocationX = table.Column<int>(type: "INTEGER", nullable: true),
                    ResultLocationY = table.Column<int>(type: "INTEGER", nullable: true),
                    ResultImage = table.Column<byte[]>(type: "BLOB", nullable: true),
                    ResultImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    ResultAccuracy = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Executions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Executions_Executions_ChildExecutionId",
                        column: x => x.ChildExecutionId,
                        principalTable: "Executions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Executions_Executions_ChildLoopExecutionId",
                        column: x => x.ChildLoopExecutionId,
                        principalTable: "Executions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Executions_FlowSteps_FlowStepId",
                        column: x => x.FlowStepId,
                        principalTable: "FlowSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Executions_Flows_FlowId",
                        column: x => x.FlowId,
                        principalTable: "Flows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Executions_ChildExecutionId",
                table: "Executions",
                column: "ChildExecutionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Executions_ChildLoopExecutionId",
                table: "Executions",
                column: "ChildLoopExecutionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Executions_FlowId",
                table: "Executions",
                column: "FlowId");

            migrationBuilder.CreateIndex(
                name: "IX_Executions_FlowStepId",
                table: "Executions",
                column: "FlowStepId");

            migrationBuilder.CreateIndex(
                name: "IX_Executions_Id",
                table: "Executions",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlowParameter_FlowId",
                table: "FlowParameter",
                column: "FlowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlowParameter_Id",
                table: "FlowParameter",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlowParameter_ParentFlowParameterId",
                table: "FlowParameter",
                column: "ParentFlowParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_Id",
                table: "Flows",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlowSteps_FlowId",
                table: "FlowSteps",
                column: "FlowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlowSteps_FlowParameterId",
                table: "FlowSteps",
                column: "FlowParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowSteps_Id",
                table: "FlowSteps",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlowSteps_ParentFlowStepId",
                table: "FlowSteps",
                column: "ParentFlowStepId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowSteps_ParentTemplateSearchFlowStepId",
                table: "FlowSteps",
                column: "ParentTemplateSearchFlowStepId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Executions");

            migrationBuilder.DropTable(
                name: "FlowSteps");

            migrationBuilder.DropTable(
                name: "FlowParameter");

            migrationBuilder.DropTable(
                name: "Flows");
        }
    }
}
