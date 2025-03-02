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
                name: "AppSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Id);
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
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Result = table.Column<string>(type: "TEXT", nullable: false),
                    RunFor = table.Column<string>(type: "TEXT", nullable: false),
                    StartedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsSelected = table.Column<bool>(type: "INTEGER", nullable: false),
                    LoopCount = table.Column<int>(type: "INTEGER", nullable: true),
                    ResultLocationX = table.Column<int>(type: "INTEGER", nullable: true),
                    ResultLocationY = table.Column<int>(type: "INTEGER", nullable: true),
                    ResultImage = table.Column<byte[]>(type: "BLOB", nullable: true),
                    ResultImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    TempResultImagePath = table.Column<string>(type: "TEXT", nullable: true),
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
                });

            migrationBuilder.CreateTable(
                name: "FlowParameters",
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
                    TemplateSearchAreaType = table.Column<string>(type: "TEXT", nullable: true),
                    ProcessName = table.Column<string>(type: "TEXT", nullable: false),
                    SystemMonitorDeviceName = table.Column<string>(type: "TEXT", nullable: false),
                    LocationTop = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationLeft = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationRight = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationBottom = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlowParameters_FlowParameters_ParentFlowParameterId",
                        column: x => x.ParentFlowParameterId,
                        principalTable: "FlowParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Flows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FlowParameterId = table.Column<int>(type: "INTEGER", nullable: false),
                    FlowStepId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentSubFlowStepId = table.Column<int>(type: "INTEGER", nullable: true),
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
                name: "FlowSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FlowId = table.Column<int>(type: "INTEGER", nullable: true),
                    SubFlowId = table.Column<int>(type: "INTEGER", nullable: true),
                    FlowParameterId = table.Column<int>(type: "INTEGER", nullable: true),
                    ParentFlowStepId = table.Column<int>(type: "INTEGER", nullable: true),
                    ParentTemplateSearchFlowStepId = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ProcessName = table.Column<string>(type: "TEXT", nullable: false),
                    IsExpanded = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSelected = table.Column<bool>(type: "INTEGER", nullable: false),
                    OrderingNum = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    TemplateMatchMode = table.Column<string>(type: "TEXT", nullable: true),
                    IsSubFlowReferenced = table.Column<bool>(type: "INTEGER", nullable: false),
                    TemplateImage = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Accuracy = table.Column<decimal>(type: "TEXT", nullable: false),
                    RemoveTemplateFromResult = table.Column<bool>(type: "INTEGER", nullable: false),
                    CursorAction = table.Column<string>(type: "TEXT", nullable: true),
                    CursorButton = table.Column<string>(type: "TEXT", nullable: true),
                    CursorScrollDirection = table.Column<string>(type: "TEXT", nullable: true),
                    CursorRelocationType = table.Column<string>(type: "TEXT", nullable: true),
                    WaitForHours = table.Column<int>(type: "INTEGER", nullable: false),
                    WaitForMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    WaitForSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    WaitForMilliseconds = table.Column<int>(type: "INTEGER", nullable: false),
                    Height = table.Column<int>(type: "INTEGER", nullable: false),
                    Width = table.Column<int>(type: "INTEGER", nullable: false),
                    IsLoop = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsLoopInfinite = table.Column<bool>(type: "INTEGER", nullable: false),
                    LoopCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LoopMaxCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LoopTime = table.Column<TimeOnly>(type: "TEXT", nullable: true),
                    LocationX = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationY = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlowSteps_FlowParameters_FlowParameterId",
                        column: x => x.FlowParameterId,
                        principalTable: "FlowParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FlowSteps_FlowSteps_ParentFlowStepId",
                        column: x => x.ParentFlowStepId,
                        principalTable: "FlowSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_FlowSteps_Flows_SubFlowId",
                        column: x => x.SubFlowId,
                        principalTable: "Flows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppSettings_Id",
                table: "AppSettings",
                column: "Id",
                unique: true);

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
                name: "IX_FlowParameters_FlowId",
                table: "FlowParameters",
                column: "FlowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlowParameters_Id",
                table: "FlowParameters",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlowParameters_ParentFlowParameterId",
                table: "FlowParameters",
                column: "ParentFlowParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_Id",
                table: "Flows",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Flows_ParentSubFlowStepId",
                table: "Flows",
                column: "ParentSubFlowStepId");

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

            migrationBuilder.CreateIndex(
                name: "IX_FlowSteps_SubFlowId",
                table: "FlowSteps",
                column: "SubFlowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Executions_FlowSteps_FlowStepId",
                table: "Executions",
                column: "FlowStepId",
                principalTable: "FlowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Executions_Flows_FlowId",
                table: "Executions",
                column: "FlowId",
                principalTable: "Flows",
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
                name: "FK_Flows_FlowSteps_ParentSubFlowStepId",
                table: "Flows",
                column: "ParentSubFlowStepId",
                principalTable: "FlowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flows_FlowSteps_ParentSubFlowStepId",
                table: "Flows");

            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "Executions");

            migrationBuilder.DropTable(
                name: "FlowSteps");

            migrationBuilder.DropTable(
                name: "FlowParameters");

            migrationBuilder.DropTable(
                name: "Flows");
        }
    }
}
