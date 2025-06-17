using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace Cpro.Forms.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class AddFormDesignHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Designer_FormDesigns_FormDesignid",
                table: "Designer");

            migrationBuilder.DropForeignKey(
                name: "FK_Processor_FormDesigns_FormDesignid",
                table: "Processor");

            migrationBuilder.DropIndex(
                name: "IX_Processor_FormDesignid",
                table: "Processor");

            migrationBuilder.DropIndex(
                name: "IX_Designer_FormDesignid",
                table: "Designer");

            migrationBuilder.DropColumn(
                name: "FormDesignid",
                table: "Processor");

            migrationBuilder.DropColumn(
                name: "FormDesignid",
                table: "Designer");

            migrationBuilder.AlterColumn<string>(
                name: "FormId",
                table: "Processor",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "FormDesigns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedOn",
                table: "FormDesigns",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "FormId",
                table: "Designer",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "FormDesignsHistory",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    FormId = table.Column<int>(type: "int", nullable: false),
                    TenantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StorageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormDesignsHistory", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "DesignerHistory",
                columns: table => new
                {
                    DesignerHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DesignerHistory", x => x.DesignerHistoryId);
                    table.ForeignKey(
                        name: "FK_DesignerHistory_FormDesignsHistory_FormId",
                        column: x => x.FormId,
                        principalTable: "FormDesignsHistory",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormStatesConfigHistory",
                columns: table => new
                {
                    FormStatesConfigHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormDesignId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormStatesConfigHistory", x => x.FormStatesConfigHistoryId);
                    table.ForeignKey(
                        name: "FK_FormStatesConfigHistory_FormDesignsHistory_FormDesignId",
                        column: x => x.FormDesignId,
                        principalTable: "FormDesignsHistory",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessorHistory",
                columns: table => new
                {
                    ProcessorHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessorHistory", x => x.ProcessorHistoryId);
                    table.ForeignKey(
                        name: "FK_ProcessorHistory_FormDesignsHistory_FormId",
                        column: x => x.FormId,
                        principalTable: "FormDesignsHistory",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Processor_FormId",
                table: "Processor",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_Designer_FormId",
                table: "Designer",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_DesignerHistory_FormId",
                table: "DesignerHistory",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_FormStatesConfigHistory_FormDesignId",
                table: "FormStatesConfigHistory",
                column: "FormDesignId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessorHistory_FormId",
                table: "ProcessorHistory",
                column: "FormId");

            migrationBuilder.AddForeignKey(
                name: "FK_Designer_FormDesigns_FormId",
                table: "Designer",
                column: "FormId",
                principalTable: "FormDesigns",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Processor_FormDesigns_FormId",
                table: "Processor",
                column: "FormId",
                principalTable: "FormDesigns",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Designer_FormDesigns_FormId",
                table: "Designer");

            migrationBuilder.DropForeignKey(
                name: "FK_Processor_FormDesigns_FormId",
                table: "Processor");

            migrationBuilder.DropTable(
                name: "DesignerHistory");

            migrationBuilder.DropTable(
                name: "FormStatesConfigHistory");

            migrationBuilder.DropTable(
                name: "ProcessorHistory");

            migrationBuilder.DropTable(
                name: "FormDesignsHistory");

            migrationBuilder.DropIndex(
                name: "IX_Processor_FormId",
                table: "Processor");

            migrationBuilder.DropIndex(
                name: "IX_Designer_FormId",
                table: "Designer");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "FormDesigns");

            migrationBuilder.DropColumn(
                name: "LastUpdatedOn",
                table: "FormDesigns");

            migrationBuilder.AlterColumn<string>(
                name: "FormId",
                table: "Processor",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "FormDesignid",
                table: "Processor",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FormId",
                table: "Designer",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "FormDesignid",
                table: "Designer",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Processor_FormDesignid",
                table: "Processor",
                column: "FormDesignid");

            migrationBuilder.CreateIndex(
                name: "IX_Designer_FormDesignid",
                table: "Designer",
                column: "FormDesignid");

            migrationBuilder.AddForeignKey(
                name: "FK_Designer_FormDesigns_FormDesignid",
                table: "Designer",
                column: "FormDesignid",
                principalTable: "FormDesigns",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Processor_FormDesigns_FormDesignid",
                table: "Processor",
                column: "FormDesignid",
                principalTable: "FormDesigns",
                principalColumn: "id");
        }
    }
}
