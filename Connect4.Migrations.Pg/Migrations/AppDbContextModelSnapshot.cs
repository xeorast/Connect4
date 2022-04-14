﻿// <auto-generated />
using System;
using Connect4.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Connect4.Migrations.Pg.Migrations
{
	[DbContext( typeof( AppDbContext ) )]
	partial class AppDbContextModelSnapshot : ModelSnapshot
	{
		protected override void BuildModel( ModelBuilder modelBuilder )
		{
#pragma warning disable 612, 618
			modelBuilder
				.HasAnnotation( "ProductVersion", "6.0.3" )
				.HasAnnotation( "Relational:MaxIdentifierLength", 63 );

			NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns( modelBuilder );

			modelBuilder.Entity( "Connect4.Domain.Models.GameModel", b =>
				 {
					 b.Property<int>( "Id" )
						 .ValueGeneratedOnAdd()
						 .HasColumnType( "integer" );

					 NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn( b.Property<int>( "Id" ) );

					 b.Property<int>( "CurrentPlayer" )
						 .HasColumnType( "integer" );

					 b.Property<int>( "NumberPlayers" )
						 .HasColumnType( "integer" );

					 b.Property<int>( "ToConnect" )
						 .HasColumnType( "integer" );

					 b.Property<Guid>( "Uuid" )
						 .HasColumnType( "uuid" );

					 b.Property<string>( "WellState" )
						 .IsRequired()
						 .HasColumnType( "text" );

					 b.Property<int?>( "Winner" )
						 .HasColumnType( "integer" );

					 b.HasKey( "Id" );

					 b.HasIndex( "Uuid" )
						 .IsUnique();

					 b.ToTable( "Games" );
				 } );
#pragma warning restore 612, 618
		}
	}
}
