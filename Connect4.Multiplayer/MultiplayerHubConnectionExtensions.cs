using Connect4.Domain.Dtos.GameEvents;
using Microsoft.AspNetCore.SignalR.Client;

namespace Connect4.Multiplayer;

public static class MultiplayerHubConnectionExtensions
{
	public delegate Task PlayerMovedHandlerNoSender( PlayerMovedDto d ); 
	public static IDisposable OnPlayerMoved( this HubConnection c, PlayerMovedHandlerNoSender h )
	{
		return c.On<PlayerMovedDto>( nameof( IOnlineGameClient.PlayerMoved), new( h ) );
	}

	public delegate Task GameEndedHandlerNoSender( GameEndedDto d );
	public static IDisposable OnGameEnded( this HubConnection c, GameEndedHandlerNoSender h )
	{
		return c.On<GameEndedDto>( nameof( IOnlineGameClient.GameEnded), new( h ) );
	}

	public delegate Task ColumnFilledHandlerNoSender( ColumnFilledDto d );
	public static IDisposable OnColumnFilled( this HubConnection c, ColumnFilledHandlerNoSender h )
	{
		return c.On<ColumnFilledDto>( nameof( IOnlineGameClient.ColumnFilled), new( h ) );
	}

	public delegate Task PlayerSwitchedHandlerNoSender( PlayerSwitchedDto d );
	public static IDisposable OnPlayerSwitched( this HubConnection c, PlayerSwitchedHandlerNoSender h )
	{
		return c.On<PlayerSwitchedDto>( nameof( IOnlineGameClient.PlayerSwitched), new( h ) );
	}

	public delegate Task TurnCompletedHandlerNoSender();
	public static IDisposable OnTurnCompleted( this HubConnection c, TurnCompletedHandlerNoSender h )
	{
		return c.On( nameof( IOnlineGameClient.TurnCompleted), new( h ) );
	}


}
