using Connect4.Domain.Dtos.GameEvents;
using Connect4.Engine;
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
		_ = await _multiplayerService.MoveAsync( uuid, column, GetEvents( uuid ) );

	}

	private IGameClient GetGameGroup( Guid uuid )
	{
		return Clients.Group( uuid.ToString() );
	}
	private GameEvents GetEvents( Guid uuid )
	{
		var clients = GetGameGroup( uuid );

		return new(
			PlayerMoved: ( Game _, int col, int row, Hue hue ) =>
				clients.PlayerMoved( new( col, row, hue ) ),

			GameEnded: ( Game _, Hue winner ) =>
				clients.GameEnded( new( winner ) ),

			ColumnFilled: ( Game _, int col ) =>
				clients.ColumnFilled( new( col ) ),

			PlayerSwitched: ( Game _, Hue oldPlayer, Hue newPlayer ) =>
				clients.PlayerSwitched( new( oldPlayer, newPlayer ) ),

			TurnCompleted: ( Game sender ) =>
				clients.TurnCompleted() );
	}

}
