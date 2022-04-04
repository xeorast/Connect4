using Connect4.Api.Exceptions;
using Connect4.Data;
using Connect4.Domain.Models;
using Connect4.Engine;
using Microsoft.EntityFrameworkCore;

namespace Connect4.Api.Services;

public interface IMultiplayerService
{
	Task<Guid> CreateGameAsync();
	Task MoveAsync( Guid uuid, int column );
	Task<GameModel?> GetBoardAsync( Guid uuid );
}

public class MultiplayerService : IMultiplayerService
{
	private readonly AppDbContext _dbContext;

	public MultiplayerService( AppDbContext dbContext )
	{
		_dbContext = dbContext;
	}

	async Task<Guid> IMultiplayerService.CreateGameAsync()
	{
		GameModel game = new( Guid.NewGuid(), new Game() );

		_ = _dbContext.Add( game );
		_ = await _dbContext.SaveChangesAsync();

		return game.Uuid;
	}

	async Task<GameModel?> IMultiplayerService.GetBoardAsync( Guid uuid )
	{
		return await GetGameAsync( uuid );
	}

	async Task IMultiplayerService.MoveAsync( Guid uuid, int column )
	{
		var gameModel = await GetGameAsync( uuid )
			?? throw new NotFoundException( "Game with given uuid not found" );

		var game = gameModel.GetGameFromState();
		_ = game.Move( column );

		gameModel.UpdateState( game );

		_ = await _dbContext.SaveChangesAsync();
	}

	private async Task<GameModel?> GetGameAsync( Guid uuid )
	{
		var game = await _dbContext.Games
			.Where( x => x.Uuid == uuid )
			.FirstOrDefaultAsync();

		return game;
	}
}
