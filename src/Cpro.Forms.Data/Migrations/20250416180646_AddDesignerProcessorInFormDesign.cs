using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace Cpro.Forms.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class AddDesignerProcessorInFormDesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Designer_FormDatas_FormDataId",
                table: "Designer");

            migrationBuilder.DropForeignKey(
                name: "FK_Processor_FormDatas_FormDataId",
                table: "Processor");

            migrationBuilder.DropIndex(
                name: "IX_Processor_FormDataId",
                table: "Processor");

            migrationBuilder.DropIndex(
                name: "IX_Designer_FormDataId",
                table: "Designer");

            migrationBuilder.DropColumn(
                name: "FormDataId",
                table: "Processor");

            migrationBuilder.DropColumn(
                name: "FormDataId",
                table: "Designer");

            migrationBuilder.AddColumn<string>(
                name: "FormDesignid",
                table: "Processor",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormId",
                table: "Processor",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FormDesignid",
                table: "Designer",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormId",
                table: "Designer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "FormId",
                table: "Processor");

            migrationBuilder.DropColumn(
                name: "FormDesignid",
                table: "Designer");

            migrationBuilder.DropColumn(
                name: "FormId",
                table: "Designer");

            migrationBuilder.AddColumn<string>(
                name: "FormDataId",
                table: "Processor",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FormDataId",
                table: "Designer",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Processor_FormDataId",
                table: "Processor",
                column: "FormDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Designer_FormDataId",
                table: "Designer",
                column: "FormDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Designer_FormDatas_FormDataId",
                table: "Designer",
                column: "FormDataId",
                principalTable: "FormDatas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Processor_FormDatas_FormDataId",
                table: "Processor",
                column: "FormDataId",
                principalTable: "FormDatas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
