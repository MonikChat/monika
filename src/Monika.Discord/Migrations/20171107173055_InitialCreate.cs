using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Monika.Discord.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatLines",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatLines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatResponses",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChatLineId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    SupportedChatTypes = table.Column<int>(type: "INTEGER", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatResponses_ChatLines_ChatLineId",
                        column: x => x.ChatLineId,
                        principalTable: "ChatLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChatTriggers",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChatLineId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    Text = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatTriggers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatTriggers_ChatLines_ChatLineId",
                        column: x => x.ChatLineId,
                        principalTable: "ChatLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatResponses_ChatLineId",
                table: "ChatResponses",
                column: "ChatLineId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatTriggers_ChatLineId",
                table: "ChatTriggers",
                column: "ChatLineId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatResponses");

            migrationBuilder.DropTable(
                name: "ChatTriggers");

            migrationBuilder.DropTable(
                name: "ChatLines");
        }
    }
}
