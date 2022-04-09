using Connect4.Domain.Core;
using Connect4.Domain.Dtos.GameEvents;
using Connect4.Multiplayer;
using Microsoft.AspNetCore.SignalR.Client;

namespace Connect4.Api.Client;

public partial class C4ApiConsumer
{
	public partial class C4ApiConsumer_Multiplayer_RealTime : C4ApiConsumer_Child, IOnlineGameClient, IDisposable
	{
		HubConnection HubConnection => hubConnection.Value;
		private readonly Lazy<HubConnection> hubConnection;
		MultiplayerRealTime_Headers Headers => headers.Value;
		private readonly Lazy<MultiplayerRealTime_Headers> headers;

		// construtor
		internal C4ApiConsumer_Multiplayer_RealTime( C4ApiConsumer api )
			: base( api )
		{
			headers = new( () => new() );
			hubConnection = new( ConfigureConnection );
		}
		private HubConnection ConfigureConnection()
		{
			var connection = new HubConnectionBuilder()
				.WithUrl(
				   Api.BaseAddress,
				   options => options.Headers = Headers )
				.Build();

			var ogc = (IOnlineGameClient)this;
			disposables.Add( connection.OnPlayerMoved( ogc.PlayerMoved ) );
			disposables.Add( connection.OnGameEnded( ogc.GameEnded ) );
			disposables.Add( connection.OnColumnFilled( ogc.ColumnFilled ) );
			disposables.Add( connection.OnPlayerSwitched( ogc.PlayerSwitched ) );
			disposables.Add( connection.OnTurnCompleted( ogc.TurnCompleted ) );

			return connection;
		}

		// connecting
		public async Task Connect()
		{
			if ( !Headers.IsGameIdSet )
			{
				throw new InvalidOperationException( "Game id not set" );
			}
			if ( !Headers.IsPlayerSet )
			{
				throw new InvalidOperationException( "Player not set" );
			}
			if ( HubConnection.State is not HubConnectionState.Disconnected )
			{
				throw new InvalidOperationException( "Already connected" );
			}

			await HubConnection.StartAsync();
		}
		public C4ApiConsumer_Multiplayer_RealTime WithGameId( Guid uuid )
		{
			Headers.GameId = uuid.ToString();
			return this;
		}
		public C4ApiConsumer_Multiplayer_RealTime PlayingAsPlayer( Hue player )
		{
			Headers.Player = player.ToString( "d" );
			return this;
		}

		// events
		public delegate Task PlayerMovedHandler( object sender, PlayerMovedDto d );
		public delegate Task GameEndedHandler( object sender, GameEndedDto d );
		public delegate Task ColumnFilledHandler( object sender, ColumnFilledDto d );
		public delegate Task PlayerSwitchedHandler( object sender, PlayerSwitchedDto d );
		public delegate Task TurnCompletedHandler( object sender );

		public event PlayerMovedHandler? PlayerMoved;
		public event GameEndedHandler? GameEnded;
		public event ColumnFilledHandler? ColumnFilled;
		public event PlayerSwitchedHandler? PlayerSwitched;
		public event TurnCompletedHandler? TurnCompleted;

		async Task IOnlineGameClient.PlayerMoved( PlayerMovedDto d )
		{
			await ( PlayerMoved?.Invoke( this, d ) ?? Task.CompletedTask ).ConfigureAwait( false );
		}
		async Task IOnlineGameClient.GameEnded( GameEndedDto d )
		{
			await ( GameEnded?.Invoke( this, d ) ?? Task.CompletedTask ).ConfigureAwait( false );
		}
		async Task IOnlineGameClient.ColumnFilled( ColumnFilledDto d )
		{
			await ( ColumnFilled?.Invoke( this, d ) ?? Task.CompletedTask ).ConfigureAwait( false );
		}
		async Task IOnlineGameClient.PlayerSwitched( PlayerSwitchedDto d )
		{
			await ( PlayerSwitched?.Invoke( this, d ) ?? Task.CompletedTask ).ConfigureAwait( false );
		}
		async Task IOnlineGameClient.TurnCompleted()
		{
			await ( TurnCompleted?.Invoke( this ) ?? Task.CompletedTask ).ConfigureAwait( false );
		}

		//actions
		public async Task Move( int column )
		{
			await HubConnection.InvokeAsync( nameof( IOnlineGameServer.Move ), column );
		}

		// dispose
		public void Dispose()
		{
			foreach ( var item in disposables )
			{
				item.Dispose();
			}
			GC.SuppressFinalize( this );
		}
		private readonly List<IDisposable> disposables = new();

	}

}
