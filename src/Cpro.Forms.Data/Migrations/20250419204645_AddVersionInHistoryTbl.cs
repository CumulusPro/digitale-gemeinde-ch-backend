using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cpro.Forms.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVersionInHistoryTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.RenameColumn(
            //    name: "ProcessorHistoryId",
            //    table: "ProcessorHistory",
            //    newName: "ProcessorId");

            //migrationBuilder.RenameColumn(
            //    name: "DesignerHistoryId",
            //    table: "DesignerHistory",
            //    newName: "FormVersion");

            migrationBuilder.AddColumn<int>(
                name: "FormVersion",
                table: "ProcessorHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FormVersion",
                table: "FormStatesConfigHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            //migrationBuilder.AddColumn<int>(
            //    name: "DesignerId",
            //    table: "DesignerHistory",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormVersion",
                table: "ProcessorHistory");

            migrationBuilder.DropColumn(
                name: "FormVersion",
                table: "FormStatesConfigHistory");

            //migrationBuilder.DropColumn(
            //    name: "DesignerId",
            //    table: "DesignerHistory");

            //migrationBuilder.RenameColumn(
            //    name: "ProcessorId",
            //    table: "ProcessorHistory",
            //    newName: "ProcessorHistoryId");

            //migrationBuilder.RenameColumn(
            //    name: "FormVersion",
            //    table: "DesignerHistory",
            //    newName: "DesignerHistoryId");
        }
    }
}
