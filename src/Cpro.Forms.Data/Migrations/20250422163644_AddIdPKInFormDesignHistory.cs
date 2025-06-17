using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace Cpro.Forms.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class AddIdPKInFormDesignHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "FormTemplates",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "FormStatesConfigHistory",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "label",
                table: "FormStatesConfigHistory",
                newName: "Label");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "FormStatesConfig",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "label",
                table: "FormStatesConfig",
                newName: "Label");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FormDesignsHistory",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FormDesigns",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FormDatas",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "FormDesignId",
                table: "FormDesignsHistory",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormDesignId",
                table: "FormDesignsHistory");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FormTemplates",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "FormStatesConfigHistory",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Label",
                table: "FormStatesConfigHistory",
                newName: "label");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "FormStatesConfig",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Label",
                table: "FormStatesConfig",
                newName: "label");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FormDesignsHistory",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FormDesigns",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FormDatas",
                newName: "id");
        }
    }
}
