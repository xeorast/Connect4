using Connect4.Domain.Core;
using Connect4.Domain.Core.GameWrappers;
using Connect4.Domain.Dtos;
using Connect4.Domain.Dtos.GameEvents;
using Connect4.Multiplayer;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using System.Net.Http.Json;

namespace Connect4;

internal class OnlineGameWrapper : GameWrapperBase, IOnlineGameClient, IDisposable
{
	private HubConnection connection;

	public override Hue this[Coordinate cord] => well[cord.Column, cord.Row];
	private Hue[,] well;
	public override Hue CurrentPlayer => currentPlayer;
	private Hue currentPlayer;
	public override Hue? Winner => winner;
	private Hue? winner;
	public override bool HasEnded => Winner is not null;

	private readonly List<IDisposable> disposables = new();


	public OnlineGameWrapper()// todo: allow parameters
	{
		well = null!;
		connection = null!;
	}

	bool wasConnected = false;
	public async Task Connect()
	{
		if ( wasConnected )
		{
			throw new InvalidOperationException( "already connected" );
		}
		wasConnected = true;

		HttpClient http = new() { BaseAddress = new( "https://localhost:7126" ) };

		var createGameResp = await http.PostAsync( "api/multiplayer", null ).ConfigureAwait( false );
		Uuid = await createGameResp.Content.ReadFromJsonAsync<Guid>().ConfigureAwait( false );
		System.Diagnostics.Debug.WriteLine( Uuid );

		var game = await http.GetFromJsonAsync<GameDto>( $"api/multiplayer/{Uuid}" ).ConfigureAwait( false )
			?? throw new Exception( "unable to create game" );

		ToConnect = game.Well.ToConnect;
		well = game.Well.Well;
		Columns = well.GetLength( 0 );
		Rows = well.GetLength( 1 );

		currentPlayer = game.CurrentPlayer;

		Players[Hue.Red] = PlayerType.Player;
		Players[Hue.Yellow] = PlayerType.Player;

		Dictionary<string, string>? headers = new()
		{
			["GameId"] = Uuid.ToString(),
			["Player"] = Hue.Red.ToString( "d" )
		};

		connection = new HubConnectionBuilder()
			.WithUrl(
				"https://localhost:7126/multiplayer",
				options => options.Headers = headers )
			.Build();

		disposables.Add( connection.OnPlayerMoved( ( (IOnlineGameClient)this ).PlayerMoved ) );
		disposables.Add( connection.OnGameEnded( ( (IOnlineGameClient)this ).GameEnded ) );
		disposables.Add( connection.OnColumnFilled( ( (IOnlineGameClient)this ).ColumnFilled ) );
		disposables.Add( connection.OnPlayerSwitched( ( (IOnlineGameClient)this ).PlayerSwitched ) );
		disposables.Add( connection.OnTurnCompleted( ( (IOnlineGameClient)this ).TurnCompleted ) );

		await connection.StartAsync().ConfigureAwait( false );
	}

	public override async void Move( int column ) // todo: make this return Task
	{
		await connection.InvokeAsync( nameof( IOnlineGameServer.Move ), column );
	}

	public override void MoveBot( TimeSpan minMoveTime )
	{
		throw new NotImplementedException(); // as for now (only two players possible) there is no point in online with bot
	}

	public override IEnumerable<Coordinate> GetWinning()
	{
		return new Well( well, ToConnect ).GetWinning().Select( c => new Coordinate { Column = c.col, Row = c.row } );
	}

	Task IOnlineGameClient.PlayerMoved( PlayerMovedDto d )
	{
		well[d.Column, d.Row] = d.Player;

		InvokePlayerMoved( d );
		return Task.CompletedTask;
	}
	Task IOnlineGameClient.GameEnded( GameEndedDto d )
	{
		winner = d.Winner;

		InvokeGameEnded( d );
		return Task.CompletedTask;
	}
	Task IOnlineGameClient.ColumnFilled( ColumnFilledDto d )
	{
		InvokeColumnFilled( d );
		return Task.CompletedTask;
	}
	Task IOnlineGameClient.PlayerSwitched( PlayerSwitchedDto d )
	{
		currentPlayer = d.NewPlayer;

		InvokePlayerSwitched( d );
		return Task.CompletedTask;
	}
	Task IOnlineGameClient.TurnCompleted()
	{
		InvokeTurnCompleted();
		return Task.CompletedTask;
	}

	public void Dispose()
	{
		foreach ( var item in disposables )
		{
			item.Dispose();
		}
		GC.SuppressFinalize( this );
	}

}
