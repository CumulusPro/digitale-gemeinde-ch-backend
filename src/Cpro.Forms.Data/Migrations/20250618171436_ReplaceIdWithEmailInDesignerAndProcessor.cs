using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace Cpro.Forms.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class ReplaceIdWithEmailInDesignerAndProcessor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessorHistoryId",
                table: "ProcessorHistory");

            migrationBuilder.DropColumn(
                name: "ProcessorId",
                table: "Processor");

            migrationBuilder.DropColumn(
                name: "DesignerHistoryId",
                table: "DesignerHistory");

            migrationBuilder.DropColumn(
                name: "DesignerId",
                table: "Designer");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ProcessorHistory",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Processor",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "DesignerHistory",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Designer",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "ProcessorHistory");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Processor");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "DesignerHistory");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Designer");

            migrationBuilder.AddColumn<int>(
                name: "ProcessorHistoryId",
                table: "ProcessorHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProcessorId",
                table: "Processor",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DesignerHistoryId",
                table: "DesignerHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DesignerId",
                table: "Designer",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
