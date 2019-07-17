using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace rabbithole.Migrations
{
    public partial class InitialDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    timestamp = table.Column<DateTime>(nullable: false),
                    exchange = table.Column<string>(nullable: true),
                    routing_key = table.Column<string>(nullable: true),
                    content = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
