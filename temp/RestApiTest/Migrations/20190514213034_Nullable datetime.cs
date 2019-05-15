using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RestApiTest.Migrations
{
    public partial class Nullabledatetime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Modified",
                table: "BlogPosts",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BlogPosts",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Modified",
                table: "BlogPosts",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BlogPosts",
                nullable: false,
                oldClrType: typeof(long))
                .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
        }
    }
}
