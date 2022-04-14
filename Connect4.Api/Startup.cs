using Connect4.Api.Hubs;
using Connect4.Data;
using Connect4.Domain.Core;
using Microsoft.OpenApi.Models;

namespace Connect4.Api;

public class Startup
{
	// Add services to the container.
	public static void ConfigureServices( WebApplicationBuilder builder )
	{
		_ = builder.Services.AddSqlServer<AppDbContext>(
			builder.Configuration.GetConnectionString( "mssqlConnection" ),
			ssob => ssob.MigrationsAssembly( "Connect4.Migrations.MsSql" ),
			ob => ob.UseLoggerFactory( LoggerFactory.Create( factoryBuilder => factoryBuilder.AddConsole() ) )
			);
		_ = builder.Services.AddScoped<IMultiplayerService, MultiplayerService>();

		_ = builder.Services.AddControllers();
		_ = builder.Services.AddSignalR();

		_ = builder.Services.AddEndpointsApiExplorer();
		_ = builder.Services.AddSwaggerGen( c =>
		  {
			  OpenApiSchema num = new() { Reference = new() { Id = "Hue", Type = ReferenceType.Schema } };
			  OpenApiSchema arr = new() { Title = "Hue array", Type = "array", Items = num };
			  c.MapType<Hue[,]>( () => new OpenApiSchema() { Title = "2D Hue array", Type = "array", Items = arr } );
		  } );
	}

	// Configure the HTTP request pipeline.
	public static void Configure( WebApplication app )
	{
		if ( app.Environment.IsDevelopment() )
		{
			_ = app.UseSwagger();
			_ = app.UseSwaggerUI();
		}

		_ = app.UseHttpsRedirection();

		_ = app.UseAuthorization();

		_ = app.MapControllers();
		_ = app.MapHub<GameHub>( "/multiplayer" );
	}
}
