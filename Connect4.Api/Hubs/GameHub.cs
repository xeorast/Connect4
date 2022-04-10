using Connect4.Api.Exceptions;
using Connect4.Domain.Core;
using Connect4.Domain.Dtos.GameEvents;
using Connect4.Multiplayer;
using Microsoft.AspNetCore.SignalR;

using PDException = Connect4.Api.Shared.Exceptions.ProblemDetailsHubException;

namespace Connect4.Api.Hubs;

public class GameHub : ExtendedHub<IOnlineGameClient>, IOnlineGameServer
{
	private readonly IMultiplayerService _multiplayerService;

	private string? GetGameId()
	{
		var id = Context.GetHttpContext()?.Request.Headers["GameId"].ToString();
		if ( string.IsNullOrEmpty( id ) )
		{
			return null;
		}
		return id;
	}
	private Guid GetGameUuid()
	{
		if ( !Guid.TryParse(
			GetGameId() ?? throw new PDException( BadRequest( "GameId header not specified." ) ),
			out var uuid ) )
		{
			throw new PDException( BadRequest( "GameId header must be a valid guid." ) );
		}

		return uuid;
	}

	private string? GetPlayerStr()
	{
		var player = Context.GetHttpContext()?.Request.Headers["Player"].ToString();
		if ( string.IsNullOrEmpty( player ) )
		{
			return null;
		}
		return player;
	}
	private Hue GetPlayer()
	{
		if ( !Enum.TryParse<Hue>(
			GetPlayerStr() ?? throw new PDException( BadRequest( "Player header not specified." ) ),
			out var player ) )
		{
			throw new PDException( BadRequest( "Player header invalid." ) );
		}

		return player;
	}

	public GameHub( IMultiplayerService multiplayerService )
	{
		_multiplayerService = multiplayerService;
	}

	public override async Task OnConnectedAsync()
	{
		var uuid = GetGameUuid();
		_ = GetPlayer();

		if ( !await _multiplayerService.DoesGameExist( uuid ) )
		{
			throw new PDException( NotFound( "Game with id specified in GameId header not found." ) );
		}

		await Groups.AddToGroupAsync( Context.ConnectionId, uuid.ToString() );
	}

	public async Task Move( int column )
	{
		var uuid = GetGameUuid();
		try
		{
			_ = await _multiplayerService.MoveAsync( uuid, column, requestingPlayer: GetPlayer(), events: GetEvents( uuid ) );
		}
		catch ( NotFoundException e )
		{
			throw new PDException( NotFound( e.Message ) );
		}
		catch ( InvalidOperationException e )
		{
			throw new PDException( BadRequest( e.Message ) );
		}
		catch ( ArgumentOutOfRangeException e )
		{
			throw new PDException( BadRequest( e.Message ) );
		}

	}

	private IOnlineGameClient GetGameGroup( Guid uuid )
	{
		return Clients.Group( uuid.ToString() );
	}
	private GameEvents GetEvents( Guid uuid )
	{
		var clients = GetGameGroup( uuid );

		return new(
			PlayerMoved: ( Game _, PlayerMovedDto d ) =>
				clients.PlayerMoved( d ),

			GameEnded: ( Game _, GameEndedDto d ) =>
				clients.GameEnded( d ),

			ColumnFilled: ( Game _, ColumnFilledDto d ) =>
				clients.ColumnFilled( d ),

			PlayerSwitched: ( Game _, PlayerSwitchedDto d ) =>
				clients.PlayerSwitched( d ),

			TurnCompleted: ( Game sender ) =>
				clients.TurnCompleted() );
	}

}
