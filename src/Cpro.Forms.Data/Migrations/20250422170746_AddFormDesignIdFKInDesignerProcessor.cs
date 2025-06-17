using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace Cpro.Forms.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class AddFormDesignIdFKInDesignerProcessor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Designer_FormDesigns_FormId",
            //    table: "Designer");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_DesignerHistory_FormDesignsHistory_FormId",
            //    table: "DesignerHistory");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Processor_FormDesigns_FormId",
            //    table: "Processor");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_ProcessorHistory_FormDesignsHistory_FormId",
            //    table: "ProcessorHistory");

            migrationBuilder.RenameColumn(
                name: "FormId",
                table: "ProcessorHistory",
                newName: "FormDesignId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_ProcessorHistory_FormId",
            //    table: "ProcessorHistory",
            //    newName: "IX_ProcessorHistory_FormDesignId");

            migrationBuilder.RenameColumn(
                name: "FormId",
                table: "Processor",
                newName: "FormDesignId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_Processor_FormId",
            //    table: "Processor",
            //    newName: "IX_Processor_FormDesignId");

            migrationBuilder.RenameColumn(
                name: "FormId",
                table: "DesignerHistory",
                newName: "FormDesignId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_DesignerHistory_FormId",
            //    table: "DesignerHistory",
            //    newName: "IX_DesignerHistory_FormDesignId");

            migrationBuilder.RenameColumn(
                name: "FormId",
                table: "Designer",
                newName: "FormDesignId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_Designer_FormId",
            //    table: "Designer",
            //    newName: "IX_Designer_FormDesignId");

            migrationBuilder.AddForeignKey(
                name: "FK_Designer_FormDesigns_FormDesignId",
                table: "Designer",
                column: "FormDesignId",
                principalTable: "FormDesigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DesignerHistory_FormDesignsHistory_FormDesignId",
                table: "DesignerHistory",
                column: "FormDesignId",
                principalTable: "FormDesignsHistory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Processor_FormDesigns_FormDesignId",
                table: "Processor",
                column: "FormDesignId",
                principalTable: "FormDesigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessorHistory_FormDesignsHistory_FormDesignId",
                table: "ProcessorHistory",
                column: "FormDesignId",
                principalTable: "FormDesignsHistory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Designer_FormDesigns_FormDesignId",
                table: "Designer");

            migrationBuilder.DropForeignKey(
                name: "FK_DesignerHistory_FormDesignsHistory_FormDesignId",
                table: "DesignerHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Processor_FormDesigns_FormDesignId",
                table: "Processor");

            migrationBuilder.DropForeignKey(
                name: "FK_ProcessorHistory_FormDesignsHistory_FormDesignId",
                table: "ProcessorHistory");

            migrationBuilder.RenameColumn(
                name: "FormDesignId",
                table: "ProcessorHistory",
                newName: "FormId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_ProcessorHistory_FormDesignId",
            //    table: "ProcessorHistory",
            //    newName: "IX_ProcessorHistory_FormId");

            migrationBuilder.RenameColumn(
                name: "FormDesignId",
                table: "Processor",
                newName: "FormId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_Processor_FormDesignId",
            //    table: "Processor",
            //    newName: "IX_Processor_FormId");

            migrationBuilder.RenameColumn(
                name: "FormDesignId",
                table: "DesignerHistory",
                newName: "FormId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_DesignerHistory_FormDesignId",
            //    table: "DesignerHistory",
            //    newName: "IX_DesignerHistory_FormId");

            migrationBuilder.RenameColumn(
                name: "FormDesignId",
                table: "Designer",
                newName: "FormId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_Designer_FormDesignId",
            //    table: "Designer",
            //    newName: "IX_Designer_FormId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Designer_FormDesigns_FormId",
            //    table: "Designer",
            //    column: "FormId",
            //    principalTable: "FormDesigns",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_DesignerHistory_FormDesignsHistory_FormId",
            //    table: "DesignerHistory",
            //    column: "FormId",
            //    principalTable: "FormDesignsHistory",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Processor_FormDesigns_FormId",
            //    table: "Processor",
            //    column: "FormId",
            //    principalTable: "FormDesigns",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_ProcessorHistory_FormDesignsHistory_FormId",
            //    table: "ProcessorHistory",
            //    column: "FormId",
            //    principalTable: "FormDesignsHistory",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }
    }
}
