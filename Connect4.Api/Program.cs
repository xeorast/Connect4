global using Connect4.Api.Services;
using Connect4.Api;

var builder = WebApplication.CreateBuilder( args );

Startup.ConfigureServices( builder );

var app = builder.Build();

Startup.Configure( app );

app.Run();
