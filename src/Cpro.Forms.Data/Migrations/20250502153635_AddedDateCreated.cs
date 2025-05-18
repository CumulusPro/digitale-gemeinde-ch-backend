using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cpro.Forms.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedDateCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "FormDesignsHistory");

            migrationBuilder.DropColumn(
                name: "LastUpdatedOn",
                table: "FormDesigns");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateCreated",
                table: "FormDesignsHistory",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateUpdated",
                table: "FormDesignsHistory",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateCreated",
                table: "FormDesigns",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateUpdated",
                table: "FormDesigns",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "FormDesignsHistory");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "FormDesignsHistory");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "FormDesigns");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "FormDesigns");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "FormDesignsHistory",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedOn",
                table: "FormDesigns",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
