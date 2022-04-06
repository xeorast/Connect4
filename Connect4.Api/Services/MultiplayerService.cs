using Connect4.Api.Exceptions;
using Connect4.Data;
using Connect4.Domain.Dtos;
using Connect4.Domain.Models;
using Connect4.Engine;
using Microsoft.EntityFrameworkCore;

namespace Connect4.Api.Services;

public interface IMultiplayerService
{
	Task<Guid> CreateGameAsync();
	/// <summary>
	/// performs move on game with given id
	/// </summary>
	/// <param name="uuid">uuid of the game</param>
	/// <param name="column">column to insert token to</param>
	/// <returns></returns>
	/// <exception cref="NotFoundException">Game not found</exception>
	/// <exception cref="ArgumentOutOfRangeException">Column out of range</exception>
	/// <exception cref="InvalidOperationException">Game ended or column is full</exception>
	Task MoveAsync( Guid uuid, int column );
	Task<GameDto?> GetBoardAsync( Guid uuid );
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

	async Task<GameDto?> IMultiplayerService.GetBoardAsync( Guid uuid )
	{
		var model = await GetGameAsync( uuid );
		if ( model is null )
		{
			return null;
		}

		var game = model.GetGameFromState();
		var well = game.CloneWell();

		WellDto wellDto = new( well.ToConnect, well.WellObj );
		return new GameDto( game.NumberPlayers, game.Winner, game.CurrentPlayer, wellDto );
	}

	/// <inheritdoc/>
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
