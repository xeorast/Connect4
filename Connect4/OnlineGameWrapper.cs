using Connect4.Domain.Core;
using Connect4.Domain.Core.GameWrappers;
using Connect4.Domain.Dtos;
using Connect4.Domain.Dtos.GameEvents;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using System.Net.Http.Json;

namespace Connect4;

// todo: merge this with the one defined in c4.Api
internal interface IGameClient
{
	Task PlayerMoved( PlayerMovedDto d );
	Task GameEnded( GameEndedDto d );
	Task ColumnFilled( ColumnFilledDto d );
	Task PlayerSwitched( PlayerSwitchedDto d );
	Task TurnCompleted();
}

internal class OnlineGameWrapper : GameWrapperBase, IGameClient, IDisposable
{
	private readonly HubConnection connection;

	public override Hue this[Coordinate cord] => well[cord.Column, cord.Row];
	private readonly Hue[,] well;
	public override Hue CurrentPlayer => currentPlayer;
	private Hue currentPlayer;
	public override Hue? Winner => winner;
	private Hue? winner;
	public override bool HasEnded => Winner is not null;

	readonly List<IDisposable> disposables;


	public OnlineGameWrapper()// todo: allow parameters
	{
		HttpClient http = new() { BaseAddress = new( "https://localhost:7126" ) };
		Uuid = http
			.PostAsync( "api/multiplayer", null )
			.GetAwaiter().GetResult()
			.Content
			.ReadFromJsonAsync<Guid>()
			.GetAwaiter().GetResult(); //todo: something to await this better
		System.Diagnostics.Debug.WriteLine( Uuid );

		var game = http.GetFromJsonAsync<GameDto>( $"api/multiplayer/{Uuid}" ).GetAwaiter().GetResult()
			?? throw new Exception( "game not created" );

		ToConnect = game.Well.ToConnect;
		well = game.Well.Well;
		Columns = well.GetLength( 0 );
		Rows = well.GetLength( 1 );


		currentPlayer = game.CurrentPlayer;

		Players[Hue.Red] = PlayerType.Player;
		Players[Hue.Yellow] = PlayerType.Player;

		connection = new HubConnectionBuilder()
			.WithUrl(
				"https://localhost:7126/multiplayer",
				options => options.Headers = new Dictionary<string, string>
				{
					["GameId"] = Uuid.ToString()
				} )
			.Build();

		disposables = new()
		{
			connection.On<PlayerMovedDto>(
				nameof( IGameClient.PlayerMoved ), ( (IGameClient)this ).PlayerMoved ),
			connection.On<GameEndedDto>(
				nameof( IGameClient.GameEnded ), ( (IGameClient)this ).GameEnded ),
			connection.On<ColumnFilledDto>(
				nameof( IGameClient.ColumnFilled ), ( (IGameClient)this ).ColumnFilled ),
			connection.On<PlayerSwitchedDto>(
				nameof( IGameClient.PlayerSwitched ), ( (IGameClient)this ).PlayerSwitched ),
			connection.On(
				nameof( IGameClient.TurnCompleted ), ( (IGameClient)this ).TurnCompleted ),
		};

		_ = connection.StartAsync();
	}

	public override async void Move( int column ) // todo: make this return Task
	{
		await connection.InvokeAsync( nameof( Move ), column );
	}

	public override void MoveBot( TimeSpan minMoveTime )
	{
		throw new NotImplementedException(); // todo: implement
	}

	public override IEnumerable<Coordinate> GetWinning()
	{
		return new Well( well, ToConnect ).GetWinning().Select( c => new Coordinate { Column = c.col, Row = c.row } );
	}

	Task IGameClient.PlayerMoved( PlayerMovedDto d )
	{
		well[d.Column, d.Row] = d.Player;

		InvokePlayerMoved( d );
		return Task.CompletedTask;
	}
	Task IGameClient.GameEnded( GameEndedDto d )
	{
		winner = d.Winner;

		InvokeGameEnded( d );
		return Task.CompletedTask;
	}
	Task IGameClient.ColumnFilled( ColumnFilledDto d )
	{
		InvokeColumnFilled( d );
		return Task.CompletedTask;
	}
	Task IGameClient.PlayerSwitched( PlayerSwitchedDto d )
	{
		currentPlayer = d.NewPlayer;

		InvokePlayerSwitched( d );
		return Task.CompletedTask;
	}
	Task IGameClient.TurnCompleted()
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
