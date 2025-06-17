using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace Cpro.Forms.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class MakeFormStatesConfigAsOwnedType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormStatesConfig_FormDesigns_FormDesignid",
                table: "FormStatesConfig");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FormStatesConfig",
                table: "FormStatesConfig");

            migrationBuilder.DropIndex(
                name: "IX_FormStatesConfig_FormDesignid",
                table: "FormStatesConfig");

            migrationBuilder.AlterColumn<string>(
                name: "FormDesignid",
                table: "FormStatesConfig",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "value",
                table: "FormStatesConfig",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "label",
                table: "FormStatesConfig",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "FormStatesConfig",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormStatesConfig",
                table: "FormStatesConfig",
                columns: new[] { "FormDesignid", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_FormStatesConfig_FormDesigns_FormDesignid",
                table: "FormStatesConfig",
                column: "FormDesignid",
                principalTable: "FormDesigns",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormStatesConfig_FormDesigns_FormDesignid",
                table: "FormStatesConfig");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FormStatesConfig",
                table: "FormStatesConfig");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "FormStatesConfig");

            migrationBuilder.AlterColumn<string>(
                name: "value",
                table: "FormStatesConfig",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "label",
                table: "FormStatesConfig",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FormDesignid",
                table: "FormStatesConfig",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormStatesConfig",
                table: "FormStatesConfig",
                columns: new[] { "label", "value" });

            migrationBuilder.CreateIndex(
                name: "IX_FormStatesConfig_FormDesignid",
                table: "FormStatesConfig",
                column: "FormDesignid");

            migrationBuilder.AddForeignKey(
                name: "FK_FormStatesConfig_FormDesigns_FormDesignid",
                table: "FormStatesConfig",
                column: "FormDesignid",
                principalTable: "FormDesigns",
                principalColumn: "id");
        }
    }
}
