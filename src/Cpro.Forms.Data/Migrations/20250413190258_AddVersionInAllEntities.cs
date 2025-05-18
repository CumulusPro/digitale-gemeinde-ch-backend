using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cpro.Forms.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVersionInAllEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "FormTemplates",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "FormDesigns",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "FormDatas",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "FormTemplates");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "FormDesigns");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "FormDatas");
        }
    }
}
