using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace Cpro.Forms.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class UpdateFormStatesConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FormStatesConfig",
                table: "FormStatesConfig");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormStatesConfig",
                table: "FormStatesConfig",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_FormStatesConfig_FormDesignid",
                table: "FormStatesConfig",
                column: "FormDesignid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FormStatesConfig",
                table: "FormStatesConfig");

            migrationBuilder.DropIndex(
                name: "IX_FormStatesConfig_FormDesignid",
                table: "FormStatesConfig");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormStatesConfig",
                table: "FormStatesConfig",
                columns: new[] { "FormDesignid", "Id" });
        }
    }
}
