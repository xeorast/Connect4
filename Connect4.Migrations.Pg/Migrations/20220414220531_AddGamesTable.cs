using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Connect4.Migrations.Pg.Migrations
{
	public partial class AddGamesTable : Migration
	{
		protected override void Up( MigrationBuilder migrationBuilder )
		{
			_ = migrationBuilder.CreateTable(
				name: "Games",
				columns: table => new
				{
					Id = table.Column<int>( type: "integer", nullable: false )
						.Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
					Uuid = table.Column<Guid>( type: "uuid", nullable: false ),
					NumberPlayers = table.Column<int>( type: "integer", nullable: false ),
					Winner = table.Column<int>( type: "integer", nullable: true ),
					CurrentPlayer = table.Column<int>( type: "integer", nullable: false ),
					ToConnect = table.Column<int>( type: "integer", nullable: false ),
					WellState = table.Column<string>( type: "text", nullable: false )
				},
				constraints: table =>
				{
					_ = table.PrimaryKey( "PK_Games", x => x.Id );
				} );

			_ = migrationBuilder.CreateIndex(
				name: "IX_Games_Uuid",
				table: "Games",
				column: "Uuid",
				unique: true );
		}

		protected override void Down( MigrationBuilder migrationBuilder )
		{
			_ = migrationBuilder.DropTable(
				name: "Games" );
		}
	}
}
