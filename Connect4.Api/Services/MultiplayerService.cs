using Connect4.Api.Exceptions;
using Connect4.Domain.Models;
using Connect4.Engine;

namespace Connect4.Api.Services;

public interface IMultiplayerService
{
	Task<Guid> CreateGameAsync();
	Task MoveAsync( Guid uuid, int column );
	Task<GameModel?> GetBoardAsync( Guid uuid );
}

public class MultiplayerService : IMultiplayerService
{
	private static readonly List<GameModel> _games = new();

	async Task<Guid> IMultiplayerService.CreateGameAsync()
	{
		await Task.CompletedTask;
		GameModel game = new( Guid.NewGuid(), new Game() );
		_games.Add( game );

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

	}

	private async Task<GameModel?> GetGameAsync( Guid uuid )
	{
		await Task.CompletedTask;
		var game = _games
			.Where( x => x.Uuid == uuid )
			.FirstOrDefault();

		return game;
	}
}
