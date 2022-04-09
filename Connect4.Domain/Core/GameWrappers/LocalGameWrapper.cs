using Connect4.Domain.Dtos.GameEvents;

namespace Connect4.Domain.Core.GameWrappers;

public class LocalGameWrapper : GameWrapperBase
{
	const int botLevel = 5;
	const int numPlayers = 2;
	private Game Game { get; set; }

	public override Hue CurrentPlayer => Game.CurrentPlayer;
	public override Hue? Winner => Game.Winner;

	public override bool HasEnded => Game.HasEnded;

	public override bool CanMove => true;

	public override Hue this[Coordinate cord] => Game.CloneWell().WellObj[cord.Column, cord.Row];

	public LocalGameWrapper( int columns, int rows, Hue startingPlayer, GameMode gameMode )
	{
		Uuid = Guid.NewGuid();

		Columns = columns;
		Rows = rows;
		StartingPlayer = startingPlayer;
		ToConnect = 4;

		Game = new Game(
			width: Columns,
			height: Rows,
			toConnect: ToConnect,
			starting: StartingPlayer,
			numberPlayers: numPlayers );

		Game.ColumnFilled += Game_ColumnFilled;
		Game.GameEnded += Game_GameEnded;
		Game.PlayerMoved += Game_PlayerMoved;
		Game.PlayerSwitched += Game_PlayerSwitched;
		Game.TurnCompleted += Game_TurnCompleted;


		Players[Hue.Red] = gameMode switch
		{
			GameMode.CvC => PlayerType.Computer,
			_ => PlayerType.Player
		};
		Players[Hue.Yellow] = gameMode switch
		{
			GameMode.PvP => PlayerType.Player,
			_ => PlayerType.Computer
		};

	}

	private void Game_ColumnFilled( Game sender, ColumnFilledDto d ) => InvokeColumnFilled( d );
	private void Game_GameEnded( Game sender, GameEndedDto d ) => InvokeGameEnded( d );
	private void Game_PlayerMoved( Game sender, PlayerMovedDto d ) => InvokePlayerMoved( d );
	private void Game_PlayerSwitched( Game sender, PlayerSwitchedDto d ) => InvokePlayerSwitched( d );
	private void Game_TurnCompleted( Game sender ) => InvokeTurnCompleted();


	public override void Move( int column )
	{
		if ( IsNowPlayer )
		{
			_ = Game.Move( column );
		}
	}

	public override async void MoveBot( TimeSpan minMoveTime )
	{
		if ( IsNowBot )
		{
			var delay = Task.Delay( minMoveTime );
			var column = Bot( Game.CurrentPlayer, botLevel ).GetRecommendation( Game.CloneWell() );
			await delay;

			_ = Game.Move( column );
		}
	}

	public override IEnumerable<Coordinate> GetWinning()
	{
		return Game.CloneWell().GetWinning().Select( c => new Coordinate { Column = c.col, Row = c.row } );
	}

	private static GameBot Bot( Hue hue, int maxLevel )
	{
		return new GameBot( hue, maxLevel, numPlayers );
	}

}
