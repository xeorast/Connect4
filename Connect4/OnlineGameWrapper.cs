using Connect4.Api.Client;
using Connect4.Domain.Core;
using Connect4.Domain.Core.GameWrappers;
using Connect4.Domain.Dtos.GameEvents;

namespace Connect4;

internal class OnlineGameWrapper : GameWrapperBase
{
	C4ApiConsumer Api { get; }

	public override Hue this[Coordinate cord] => well[cord.Column, cord.Row];
	public override Hue CurrentPlayer => currentPlayer;
	public override Hue? Winner => winner;
	public override bool CanMove => BoundPlayer == currentPlayer && wasMoveCompleted;

	private Hue[,] well;
	private Hue currentPlayer;
	private Hue? winner;
	private bool wasMoveCompleted = true;

	public Hue BoundPlayer { get; set; }

	public OnlineGameWrapper( Hue player )
	{
		well = null!;
		Api = new( "https://localhost:7126" );
		BoundPlayer = player;
	}

	bool wasConnected = false;
	public async Task Connect( Guid uuid )
	{
		if ( wasConnected )
		{
			throw new InvalidOperationException( "already connected" );
		}

		Uuid = uuid;

		var game = await Api.Multiplayer.GetBoard( uuid ).ConfigureAwait( false )
			?? throw new NullReferenceException( "game not found" );

		ToConnect = game.Well.ToConnect;
		well = game.Well.Well;
		Columns = well.GetLength( 0 );
		Rows = well.GetLength( 1 );

		currentPlayer = game.CurrentPlayer;

		Players[Hue.Red] = PlayerType.Player;
		Players[Hue.Yellow] = PlayerType.Player;

		Api.RealTimeMultiplayer.PlayerMoved += RealTimeMultiplayer_PlayerMoved;
		Api.RealTimeMultiplayer.GameEnded += RealTimeMultiplayer_GameEnded;
		Api.RealTimeMultiplayer.ColumnFilled += RealTimeMultiplayer_ColumnFilled;
		Api.RealTimeMultiplayer.PlayerSwitched += RealTimeMultiplayer_PlayerSwitched;
		Api.RealTimeMultiplayer.TurnCompleted += RealTimeMultiplayer_TurnCompleted;

		await Api.RealTimeMultiplayer
			.WithGameId( uuid )
			.PlayingAsPlayer( BoundPlayer )
			.ConnectAsync()
			.ConfigureAwait( false );

		wasConnected = true;
	}

	public override async void Move( int column ) // todo: make this return Task
	{
		wasMoveCompleted = false;
		await Api.RealTimeMultiplayer.Move( column ).ConfigureAwait( false );
		wasMoveCompleted = true;
	}
	public override void MoveBot( TimeSpan minMoveTime ) => throw new NotImplementedException(); // as for now (only two players possible) there is no point in online with bot
	public override IEnumerable<Coordinate> GetWinning()
	{
		return new Well( well, ToConnect )
			.GetWinning()
			.Select( c => (Coordinate)c );
	}

	private Task RealTimeMultiplayer_PlayerMoved( object sender, PlayerMovedDto d )
	{
		well[d.Column, d.Row] = d.Player;

		InvokePlayerMoved( d );
		return Task.CompletedTask;
	}
	private Task RealTimeMultiplayer_GameEnded( object sender, GameEndedDto d )
	{
		winner = d.Winner;

		InvokeGameEnded( d );
		return Task.CompletedTask;
	}
	private Task RealTimeMultiplayer_ColumnFilled( object sender, ColumnFilledDto d )
	{
		InvokeColumnFilled( d );
		return Task.CompletedTask;
	}
	private Task RealTimeMultiplayer_PlayerSwitched( object sender, PlayerSwitchedDto d )
	{
		currentPlayer = d.NewPlayer;

		InvokePlayerSwitched( d );
		return Task.CompletedTask;
	}
	private Task RealTimeMultiplayer_TurnCompleted( object sender )
	{
		InvokeTurnCompleted();
		return Task.CompletedTask;
	}

}
