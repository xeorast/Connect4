using Connect4.Api.Exceptions;
using Connect4.Domain.Core;
using Connect4.Domain.Dtos.GameEvents;
using Microsoft.AspNetCore.SignalR;

namespace Connect4.Api.Hubs;

public interface IGameClient
{
	Task PlayerMoved( PlayerMovedDto move );
	Task GameEnded( GameEndedDto d );
	Task ColumnFilled( ColumnFilledDto d );
	Task PlayerSwitched( PlayerSwitchedDto d );
	Task TurnCompleted();
}

public class GameHub : Hub<IGameClient>
{
	private readonly IMultiplayerService _multiplayerService;

	public string? GetGameId()
	{
		var id = Context.GetHttpContext()?.Request.Headers["GameId"].ToString();
		if ( string.IsNullOrEmpty( id ) )
		{
			return null;
		}
		return id;
	}
	public Guid GetGameUuid()
	{
		return Guid.Parse( GetGameId() ?? throw new NullReferenceException( "game id not specified" ) );
	}

	public GameHub( IMultiplayerService multiplayerService )
	{
		_multiplayerService = multiplayerService;
	}

	public override async Task OnConnectedAsync()
	{
		var gameId = GetGameId();
		if ( gameId is null )
		{
			throw new HubException( "GameId header is required." );
		}
		if ( !Guid.TryParse( gameId, out var uuid ) )
		{
			throw new HubException( "GameId header must be a valid guid." );
		}
		if ( !await _multiplayerService.DoesGameExist( uuid ) )
		{
			throw new HubException( "Game with id specified in GameId header not found." );
		}

		await Groups.AddToGroupAsync( Context.ConnectionId, gameId );

	}

	public async Task Move( int column )
	{
		var uuid = GetGameUuid();
		try
		{
			_ = await _multiplayerService.MoveAsync( uuid, column, GetEvents( uuid ) );
		}
		catch ( NotFoundException e )
		{
			throw new HubException( e.Message );
		}
		catch ( InvalidOperationException e )
		{
			throw new HubException( e.Message );
		}
		catch ( ArgumentOutOfRangeException e )
		{
			throw new HubException( e.Message );
		}

	}

	private IGameClient GetGameGroup( Guid uuid )
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
