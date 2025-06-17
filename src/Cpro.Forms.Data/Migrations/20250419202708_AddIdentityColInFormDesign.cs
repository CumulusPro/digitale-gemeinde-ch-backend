using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace Cpro.Forms.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class AddIdentityColInFormDesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormStatesConfig_FormDesigns_FormDesignid",
                table: "FormStatesConfig");

            migrationBuilder.DropTable(
                name: "ProcessorHistory");

            migrationBuilder.DropTable(
                name: "Processor");

            migrationBuilder.DropTable(
                name: "DesignerHistory");

            migrationBuilder.DropTable(
                name: "Designer");

            migrationBuilder.RenameColumn(
                name: "FormDesignid",
                table: "FormStatesConfig",
                newName: "FormDesignId");

            migrationBuilder.RenameIndex(
                name: "IX_FormStatesConfig_FormDesignid",
                table: "FormStatesConfig",
                newName: "IX_FormStatesConfig_FormDesignId");

            // Recreate Designer table
            migrationBuilder.CreateTable(
                name: "Designer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                              .Annotation("SqlServer:Identity", "1, 1"),
                    DesignerId = table.Column<int>(nullable: false),
                    FormId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Designer", x => x.Id);
                });

            // Recreate Processor table
            migrationBuilder.CreateTable(
                name: "Processor",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                              .Annotation("SqlServer:Identity", "1, 1"),
                    ProcessorId = table.Column<int>(nullable: false),
                    FormId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processor", x => x.Id);
                });

            // Recreate DesignerHistory table
            migrationBuilder.CreateTable(
                name: "DesignerHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                              .Annotation("SqlServer:Identity", "1, 1"),
                    DesignerHistoryId = table.Column<int>(nullable: false),
                    FormId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DesignerHistory", x => x.Id);
                });

            // Recreate ProcessorHistory table
            migrationBuilder.CreateTable(
                name: "ProcessorHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                              .Annotation("SqlServer:Identity", "1, 1"),
                    ProcessorHistoryId = table.Column<int>(nullable: false),
                    FormId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessorHistory", x => x.Id);
                });

            // Recreate foreign key
            migrationBuilder.AddForeignKey(
                name: "FK_FormStatesConfig_FormDesigns_FormDesignId",
                table: "FormStatesConfig",
                column: "FormDesignId",
                principalTable: "FormDesigns",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormStatesConfig_FormDesigns_FormDesignId",
                table: "FormStatesConfig");

            migrationBuilder.DropTable(name: "Designer");
            migrationBuilder.DropTable(name: "Processor");
            migrationBuilder.DropTable(name: "DesignerHistory");
            migrationBuilder.DropTable(name: "ProcessorHistory");

            migrationBuilder.RenameColumn(
                name: "FormDesignId",
                table: "FormStatesConfig",
                newName: "FormDesignid");

            migrationBuilder.RenameIndex(
                name: "IX_FormStatesConfig_FormDesignId",
                table: "FormStatesConfig",
                newName: "IX_FormStatesConfig_FormDesignid");

            // Recreate original Designer table
            migrationBuilder.CreateTable(
                name: "Designer",
                columns: table => new
                {
                    DesignerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Designer", x => x.DesignerId);
                });

            // Recreate original Processor table
            migrationBuilder.CreateTable(
                name: "Processor",
                columns: table => new
                {
                    ProcessorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processor", x => x.ProcessorId);
                });

            // Recreate original DesignerHistory table
            migrationBuilder.CreateTable(
                name: "DesignerHistory",
                columns: table => new
                {
                    DesignerHistoryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DesignerId = table.Column<int>(nullable: false),
                    FormId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DesignerHistory", x => x.DesignerHistoryId);
                });

            // Recreate original ProcessorHistory table
            migrationBuilder.CreateTable(
                name: "ProcessorHistory",
                columns: table => new
                {
                    ProcessorHistoryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcessorId = table.Column<int>(nullable: false),
                    FormId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessorHistory", x => x.ProcessorHistoryId);
                });

            // Recreate old foreign key
            migrationBuilder.AddForeignKey(
                name: "FK_FormStatesConfig_FormDesigns_FormDesignid",
                table: "FormStatesConfig",
                column: "FormDesignid",
                principalTable: "FormDesigns",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
