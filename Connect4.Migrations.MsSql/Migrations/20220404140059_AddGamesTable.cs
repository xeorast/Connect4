using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Connect4.Migrations.MsSql.Migrations
{
	public partial class AddGamesTable : Migration
	{
		protected override void Up( MigrationBuilder migrationBuilder )
		{
			_ = migrationBuilder.CreateTable(
				name: "Games",
				columns: table => new
				{
					Id = table.Column<int>( type: "int", nullable: false )
						.Annotation( "SqlServer:Identity", "1, 1" ),
					Uuid = table.Column<Guid>( type: "uniqueidentifier", nullable: false ),
					State = table.Column<string>( type: "nvarchar(max)", nullable: false )
				},
				constraints: table =>
				{
					_ = table.PrimaryKey( "PK_Games", x => x.Id );
				} );

			_ = migrationBuilder.CreateIndex(
				name: "IX_Games_Uuid",
				table: "Games",
				column: "Uuid" );
		}

		protected override void Down( MigrationBuilder migrationBuilder )
		{
			_ = migrationBuilder.DropTable(
				name: "Games" );
		}
	}
}
