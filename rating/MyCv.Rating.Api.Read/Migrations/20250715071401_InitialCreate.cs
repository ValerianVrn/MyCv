using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCv.Rating.Api.Read.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assessment",
                columns: table => new
                {
                    EntityGuid = table.Column<Guid>(type: "uniqueidentifier", maxLength: 36, nullable: false),
                    VisitorId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Recommend = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assessment", x => x.EntityGuid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assessment_EntityGuid",
                table: "Assessment",
                column: "EntityGuid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assessment");
        }
    }
}
