using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VAST.Ontology.Database.Migrations
{
    public partial class AddAnnotationCreationInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Annotations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Annotations",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "Annotations");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Annotations");
        }
    }
}
