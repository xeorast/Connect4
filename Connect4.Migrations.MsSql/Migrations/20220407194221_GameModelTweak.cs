using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Connect4.Migrations.MsSql.Migrations
{
	// table must be empty to apply this migration
	public partial class GameModelTweak : Migration
	{
		protected override void Up( MigrationBuilder migrationBuilder )
		{

			_ = migrationBuilder.DropColumn(
				name: "State",
				table: "Games" );

			_ = migrationBuilder.AddColumn<string>(
				name: "WellState",
				table: "Games",
				type: "nvarchar(max)",
				nullable: false );

			_ = migrationBuilder.AddColumn<int>(
				name: "CurrentPlayer",
				table: "Games",
				type: "int",
				nullable: false,
				defaultValue: 0 );

			_ = migrationBuilder.AddColumn<int>(
				name: "NumberPlayers",
				table: "Games",
				type: "int",
				nullable: false,
				defaultValue: 0 );

			_ = migrationBuilder.AddColumn<int>(
				name: "ToConnect",
				table: "Games",
				type: "int",
				nullable: false,
				defaultValue: 0 );

			_ = migrationBuilder.AddColumn<int>(
				name: "Winner",
				table: "Games",
				type: "int",
				nullable: true );
		}

		protected override void Down( MigrationBuilder migrationBuilder )
		{
			_ = migrationBuilder.DropColumn(
				name: "CurrentPlayer",
				table: "Games" );

			_ = migrationBuilder.DropColumn(
				name: "NumberPlayers",
				table: "Games" );

			_ = migrationBuilder.DropColumn(
				name: "ToConnect",
				table: "Games" );

			_ = migrationBuilder.DropColumn(
				name: "Winner",
				table: "Games" );

			_ = migrationBuilder.DropColumn(
				name: "WellState",
				table: "Games" );

			_ = migrationBuilder.AddColumn<string>(
				name: "State",
				table: "Games",
				type: "nvarchar(max)",
				nullable: false );
		}
	}
}
