using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Connect4.Migrations.MsSql.Migrations
{
    public partial class UniqueGameUuidIx : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.DropIndex(
				name: "IX_Games_Uuid",
				table: "Games" );

			_ = migrationBuilder.CreateIndex(
				name: "IX_Games_Uuid",
				table: "Games",
				column: "Uuid",
				unique: true );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.DropIndex(
				name: "IX_Games_Uuid",
				table: "Games" );

			_ = migrationBuilder.CreateIndex(
				name: "IX_Games_Uuid",
				table: "Games",
				column: "Uuid" );
        }
    }
}
