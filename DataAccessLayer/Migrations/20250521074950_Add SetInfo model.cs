using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddSetInfomodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InfoMessage",
                table: "Sets");

            migrationBuilder.CreateTable(
                name: "SetInfo",
                columns: table => new
                {
                    SetInfoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InfoMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPlayer1Serve = table.Column<bool>(type: "bit", nullable: false),
                    IsPlayer1StartServer = table.Column<bool>(type: "bit", nullable: false),
                    SetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetInfo", x => x.SetInfoId);
                    table.ForeignKey(
                        name: "FK_SetInfo_Sets_SetId",
                        column: x => x.SetId,
                        principalTable: "Sets",
                        principalColumn: "SetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SetInfo_SetId",
                table: "SetInfo",
                column: "SetId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SetInfo");

            migrationBuilder.AddColumn<string>(
                name: "InfoMessage",
                table: "Sets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
