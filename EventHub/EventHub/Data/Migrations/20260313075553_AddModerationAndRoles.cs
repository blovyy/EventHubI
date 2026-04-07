using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHub.Migrations
{
    /// <inheritdoc />
    public partial class AddModerationAndRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ModeratedAt",
                table: "Events",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModeratedById",
                table: "Events",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModerationStatus",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Events_ModeratedById",
                table: "Events",
                column: "ModeratedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_ModeratedById",
                table: "Events",
                column: "ModeratedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_ModeratedById",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_ModeratedById",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ModeratedAt",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ModeratedById",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ModerationStatus",
                table: "Events");
        }
    }
}
