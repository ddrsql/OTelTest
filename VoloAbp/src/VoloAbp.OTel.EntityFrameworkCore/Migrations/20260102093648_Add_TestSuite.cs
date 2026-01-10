using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoloAbp.OTel.Migrations
{
    /// <inheritdoc />
    public partial class Add_TestSuite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestSuites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    ProjectKey = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Version = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValue: "1.0.0"),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastExecutionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    AverageExecutionTimeTicks = table.Column<long>(type: "bigint", nullable: true),
                    ExtraProperties = table.Column<string>(type: "longtext", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatorId = table.Column<Guid>(type: "char(36)", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "char(36)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "char(36)", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestSuites", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TestCases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    TestSuiteId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    Steps = table.Column<string>(type: "longtext", nullable: false),
                    ExpectedResult = table.Column<string>(type: "longtext", nullable: false),
                    ActualResult = table.Column<string>(type: "longtext", nullable: true),
                    IsEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    PriorityValue = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastRunTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ExecutionDurationTicks = table.Column<long>(type: "bigint", nullable: true),
                    ErrorMessage = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCases", x => new { x.Id, x.TestSuiteId });
                    table.ForeignKey(
                        name: "FK_TestCases_TestSuites_TestSuiteId",
                        column: x => x.TestSuiteId,
                        principalTable: "TestSuites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TestSuiteConfigurations",
                columns: table => new
                {
                    TestSuiteId = table.Column<Guid>(type: "char(36)", nullable: false),
                    TimeoutInSeconds = table.Column<int>(type: "int", nullable: false, defaultValue: 30),
                    MaxRetryCount = table.Column<int>(type: "int", nullable: false, defaultValue: 3),
                    EnableParallelExecution = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    Environment = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, defaultValue: "Development")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestSuiteConfigurations", x => x.TestSuiteId);
                    table.ForeignKey(
                        name: "FK_TestSuiteConfigurations_TestSuites_TestSuiteId",
                        column: x => x.TestSuiteId,
                        principalTable: "TestSuites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TestCases_LastRunTime",
                table: "TestCases",
                column: "LastRunTime");

            migrationBuilder.CreateIndex(
                name: "IX_TestCases_Status",
                table: "TestCases",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TestCases_TestSuiteId",
                table: "TestCases",
                column: "TestSuiteId");

            migrationBuilder.CreateIndex(
                name: "IX_TestCases_Title",
                table: "TestCases",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_TestSuites_LastExecutionTime",
                table: "TestSuites",
                column: "LastExecutionTime");

            migrationBuilder.CreateIndex(
                name: "IX_TestSuites_Name",
                table: "TestSuites",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TestSuites_Name_ProjectKey",
                table: "TestSuites",
                columns: new[] { "Name", "ProjectKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestSuites_ProjectKey",
                table: "TestSuites",
                column: "ProjectKey");

            migrationBuilder.CreateIndex(
                name: "IX_TestSuites_Status",
                table: "TestSuites",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestCases");

            migrationBuilder.DropTable(
                name: "TestSuiteConfigurations");

            migrationBuilder.DropTable(
                name: "TestSuites");
        }
    }
}
