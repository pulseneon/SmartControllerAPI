using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace asp_net_db.Migrations
{
    /// <inheritdoc />
    public partial class backupfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServerId",
                table: "Backups",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServerId",
                table: "Backups");
        }
    }
}
