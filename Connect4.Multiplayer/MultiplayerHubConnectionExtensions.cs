using Connect4.Domain.Dtos.GameEvents;
using Microsoft.AspNetCore.SignalR.Client;

namespace Connect4.Multiplayer;

public static class MultiplayerHubConnectionExtensions
{
	public static IDisposable OnPlayerMoved( this HubConnection c, IOnlineGameClient.PlayerMovedHandler h )
	{
		return c.On<PlayerMovedDto>( nameof( IOnlineGameClient.PlayerMoved), new( h ) );
	}
	public static IDisposable OnGameEnded( this HubConnection c, IOnlineGameClient.GameEndedHandler h )
	{
		return c.On<GameEndedDto>( nameof( IOnlineGameClient.GameEnded), new( h ) );
	}
	public static IDisposable OnColumnFilled( this HubConnection c, IOnlineGameClient.ColumnFilledHandler h )
	{
		return c.On<ColumnFilledDto>( nameof( IOnlineGameClient.ColumnFilled), new( h ) );
	}
	public static IDisposable OnPlayerSwitched( this HubConnection c, IOnlineGameClient.PlayerSwitchedHandler h )
	{
		return c.On<PlayerSwitchedDto>( nameof( IOnlineGameClient.PlayerSwitched), new( h ) );
	}
	public static IDisposable OnTurnCompleted( this HubConnection c, IOnlineGameClient.TurnCompletedHandler h )
	{
		return c.On( nameof( IOnlineGameClient.TurnCompleted), new( h ) );
	}


}
