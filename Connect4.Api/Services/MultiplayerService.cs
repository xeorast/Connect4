﻿using Connect4.Api.Exceptions;
using Connect4.Data;
using Connect4.Domain.Dtos;
using Connect4.Domain.Dtos.GameEvents;
using Connect4.Domain.Models;
using Connect4.Engine;
using Microsoft.EntityFrameworkCore;

namespace Connect4.Api.Services;

public record GameEvents(
	Game.PlayerMovedEventHandler? PlayerMoved = null,
	Game.GameEndedEventHandler? GameEnded = null,
	Game.ColumnFilledEventHandler? ColumnFilled = null,
	Game.PlayerSwitchedEventHandler? PlayerSwitched = null,
	Game.TurnCompletedEventHandler? TurnCompleted = null );

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
	Task<PlayerMovedDto> MoveAsync( Guid uuid, int column, GameEvents? events = null );
	Task<GameDto?> GetBoardAsync( Guid uuid );
	Task<bool> DoesGameExist( Guid uuid );
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
	async Task<PlayerMovedDto> IMultiplayerService.MoveAsync( Guid uuid, int column, GameEvents? events )
	{
		var gameModel = await GetGameAsync( uuid )
			?? throw new NotFoundException( "Game with given uuid not found" );

		var game = gameModel.GetGameFromState();
		BindEvents( game, events );


		var player = game.CurrentPlayer;
		var row = game.Move( column );

		gameModel.UpdateState( game );
		_ = await _dbContext.SaveChangesAsync();

		return new PlayerMovedDto( column, row, player );
	}

	async Task<bool> IMultiplayerService.DoesGameExist( Guid uuid )
	{
		return await _dbContext.Games
			.Where( x => x.Uuid == uuid )
			.AnyAsync();
	}

	private async Task<GameModel?> GetGameAsync( Guid uuid )
	{
		var game = await _dbContext.Games
			.Where( x => x.Uuid == uuid )
			.FirstOrDefaultAsync();

		return game;
	}

	private static void BindEvents( Game game, GameEvents? events )
	{
		if ( events is null )
		{
			return;
		}

		game.PlayerMoved += events.PlayerMoved;
		game.GameEnded += events.GameEnded;
		game.ColumnFilled += events.ColumnFilled;
		game.PlayerSwitched += events.PlayerSwitched;
		game.TurnCompleted += events.TurnCompleted;
	}

}
