using Microsoft.EntityFrameworkCore.Migrations;

namespace RubiconTest.Migrations
{
    public partial class EditPrimaryKeyAndAutoIncrement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Tags",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "updatedAt",
                table: "BlogPosts",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "BlogPosts",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "tagList",
                table: "BlogPosts",
                newName: "TagList");

            migrationBuilder.RenameColumn(
                name: "slug",
                table: "BlogPosts",
                newName: "Slug");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "BlogPosts",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "createdAt",
                table: "BlogPosts",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "body",
                table: "BlogPosts",
                newName: "Body");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BlogPosts",
                newName: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Tags",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "BlogPosts",
                newName: "updatedAt");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "BlogPosts",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "TagList",
                table: "BlogPosts",
                newName: "tagList");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "BlogPosts",
                newName: "slug");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "BlogPosts",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "BlogPosts",
                newName: "createdAt");

            migrationBuilder.RenameColumn(
                name: "Body",
                table: "BlogPosts",
                newName: "body");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "BlogPosts",
                newName: "Id");
        }
    }
}
