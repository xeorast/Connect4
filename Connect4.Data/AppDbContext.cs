using Connect4.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Connect4.Data;

public class AppDbContext : DbContext
{
	public AppDbContext( DbContextOptions options )
		: base( options )
	{
		ArgumentNullException.ThrowIfNull( Games );
	}

	public DbSet<GameModel> Games { get; set; }

}
