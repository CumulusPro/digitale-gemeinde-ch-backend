using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cpro.Forms.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDesignerAndProcessorInFormData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StorageUrl",
                table: "FormDesigns",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Designer",
                columns: table => new
                {
                    DesignerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormDataId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Designer", x => x.DesignerId);
                    table.ForeignKey(
                        name: "FK_Designer_FormDatas_FormDataId",
                        column: x => x.FormDataId,
                        principalTable: "FormDatas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Processor",
                columns: table => new
                {
                    ProcessorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormDataId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processor", x => x.ProcessorId);
                    table.ForeignKey(
                        name: "FK_Processor_FormDatas_FormDataId",
                        column: x => x.FormDataId,
                        principalTable: "FormDatas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Designer_FormDataId",
                table: "Designer",
                column: "FormDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Processor_FormDataId",
                table: "Processor",
                column: "FormDataId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Designer");

            migrationBuilder.DropTable(
                name: "Processor");

            migrationBuilder.AlterColumn<string>(
                name: "StorageUrl",
                table: "FormDesigns",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
