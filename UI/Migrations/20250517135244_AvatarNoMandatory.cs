using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSS.UI.Migrations
{
    /// <inheritdoc />
    public partial class AvatarNoMandatory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Avatar",
                table: "AspNetUsers",
                type: "BLOB",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MimeType",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MimeType",
                table: "AspNetUsers");
        }
    }
}
