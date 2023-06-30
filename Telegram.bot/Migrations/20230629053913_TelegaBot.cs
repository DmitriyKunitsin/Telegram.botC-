using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BOT.Migrations
{
    /// <inheritdoc />
    public partial class TelegaBot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    ChatID = table.Column<long>(type: "bigint", nullable: false),
                    LastMessageTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    notificationEn = table.Column<bool>(type: "bit", nullable: false),
                    phoneNum = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
