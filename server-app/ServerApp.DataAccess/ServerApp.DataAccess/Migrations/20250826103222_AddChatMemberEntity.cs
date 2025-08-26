using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerApp.DataAccess.ServerApp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddChatMemberEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_AspNetUsers_OwnerId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_ChatImages_ImageId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_ImageId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_OwnerId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "MemberIDs",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Chats");

            migrationBuilder.CreateTable(
                name: "ChatMembers",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ChatId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMembers", x => new { x.ChatId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ChatMembers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMembers_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMembers_UserId",
                table: "ChatMembers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMembers");

            migrationBuilder.AddColumn<List<string>>(
                name: "MemberIDs",
                table: "Chats",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Chats",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_ImageId",
                table: "Chats",
                column: "ImageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_OwnerId",
                table: "Chats",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_AspNetUsers_OwnerId",
                table: "Chats",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_ChatImages_ImageId",
                table: "Chats",
                column: "ImageId",
                principalTable: "ChatImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
