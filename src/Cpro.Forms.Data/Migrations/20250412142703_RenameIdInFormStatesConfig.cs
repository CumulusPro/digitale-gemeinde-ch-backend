using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cpro.Forms.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameIdInFormStatesConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FormStatesConfig",
                newName: "FormStatesConfigId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FormStatesConfigId",
                table: "FormStatesConfig",
                newName: "Id");
        }
    }
}
