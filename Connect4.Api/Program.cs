using Connect4.Api.Services;
using Connect4.Engine;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder( args );

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( c =>
{
	OpenApiSchema num = new() { Reference = new() { Id = "Hue", Type = ReferenceType.Schema } };
	OpenApiSchema arr = new() { Title = "Hue array", Type = "array", Items = num };
	c.MapType<Hue[,]>( () => new OpenApiSchema() { Title = "2D Hue array", Type = "array", Items = arr } );
} );

builder.Services.AddScoped<IMultiplayerService, MultiplayerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment() )
{
	_ = app.UseSwagger();
	_ = app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
