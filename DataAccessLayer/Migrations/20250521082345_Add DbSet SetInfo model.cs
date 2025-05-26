using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddDbSetSetInfomodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SetInfo_Sets_SetId",
                table: "SetInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SetInfo",
                table: "SetInfo");

            migrationBuilder.RenameTable(
                name: "SetInfo",
                newName: "SetInfos");

            migrationBuilder.RenameIndex(
                name: "IX_SetInfo_SetId",
                table: "SetInfos",
                newName: "IX_SetInfos_SetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SetInfos",
                table: "SetInfos",
                column: "SetInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_SetInfos_Sets_SetId",
                table: "SetInfos",
                column: "SetId",
                principalTable: "Sets",
                principalColumn: "SetId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SetInfos_Sets_SetId",
                table: "SetInfos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SetInfos",
                table: "SetInfos");

            migrationBuilder.RenameTable(
                name: "SetInfos",
                newName: "SetInfo");

            migrationBuilder.RenameIndex(
                name: "IX_SetInfos_SetId",
                table: "SetInfo",
                newName: "IX_SetInfo_SetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SetInfo",
                table: "SetInfo",
                column: "SetInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_SetInfo_Sets_SetId",
                table: "SetInfo",
                column: "SetId",
                principalTable: "Sets",
                principalColumn: "SetId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
