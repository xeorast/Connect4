using System.Diagnostics.CodeAnalysis;

namespace Connect4.Engine;

public class Game
{
	Well Well { get; }
	public int Width => Well.Width;
	public int Height => Well.Height;

	public int NumberPlayers { get; }

	public Hue? Winner { get; private set; }

	[MemberNotNullWhen( true, nameof( Winner ) )]
	public bool HasEnded => Winner is not null;

	public Hue CurrentPlayer { get; private set; }
	public Hue NextPlayer => CurrentPlayer.Next( NumberPlayers );

	/// <summary>
	/// creates game with given parameters
	/// </summary>
	/// <param name="width">number of columns in well</param>
	/// <param name="height">number of rows in well</param>
	/// <param name="toConnect">numbers of token required for win</param>
	/// <param name="numberPlayers">number of players</param>
	/// <param name="starting">starting player</param>
	public Game( int width = 7, int height = 6, int toConnect = 4, int numberPlayers = 2, Hue starting = Hue.Red )
	{
		Well = new( width, height, toConnect );
		NumberPlayers = numberPlayers;
		CurrentPlayer = starting;
	}

	/// <summary>
	/// handler for <see cref="PlayerMoved"/> event
	/// </summary>
	/// <param name="sender">game instance calling this event</param>
	/// <param name="col">column where token was placed</param>
	/// <param name="row">row where token was placed</param>
	/// <param name="hue">color of placed token</param>
	public delegate void PlayerMovedEventHandler( Game sender, int col, int row, Hue hue );
	/// <summary>
	/// called when player moves
	/// </summary>
	public event PlayerMovedEventHandler? PlayerMoved;
	/// <summary>
	/// handler for <see cref="GameEnded"/> event
	/// </summary>
	/// <param name="sender">game instance calling this event</param>
	/// <param name="winner">game winner</param>
	public delegate void GameEndedEventHandler( Game sender, Hue winner );
	/// <summary>
	/// called when game ends, either with one winner or a draw
	/// </summary>
	public event GameEndedEventHandler? GameEnded;
	/// <summary>
	/// handler for <see cref="ColumnFilled"/> event
	/// </summary>
	/// <param name="sender">game instance calling this event</param>
	/// <param name="col">column that got filled</param>
	public delegate void ColumnFilledEventHandler( Game sender, int col );
	/// <summary>
	/// called when column gets fully filled
	/// </summary>
	public event ColumnFilledEventHandler? ColumnFilled;
	/// <summary>
	/// handler for <see cref="PlayerSwitched"/> event
	/// </summary>
	/// <param name="sender">game instance calling this event</param>
	/// <param name="oldPlayer">previous player</param>
	/// <param name="newPlayer">current player</param>
	public delegate void PlayerSwitchedEventHandler( Game sender, Hue oldPlayer, Hue newPlayer );
	/// <summary>
	/// called when current player is switched
	/// </summary>
	public event PlayerSwitchedEventHandler? PlayerSwitched;
	/// <summary>
	/// handler for <see cref="TurnCompleted"/> event
	/// </summary>
	/// <param name="sender">game instance calling this event</param>
	public delegate void TurnCompletedEventHandler( Game sender );
	/// <summary>
	/// called when player move is fully handled
	/// </summary>
	public event TurnCompletedEventHandler? TurnCompleted;

	/// <summary>
	/// inserts token into given column
	/// </summary>
	/// <param name="col">column to insert token to</param>
	/// <returns>row where token got inserted</returns>
	public int Move( int col )
	{
		int row = Well.InsertToken( col, CurrentPlayer );

		PlayerMoved?.Invoke( this, col, row, CurrentPlayer );

		if ( Well.IsColumnFull( col ) )
		{
			ColumnFilled?.Invoke( this, col );
		}

		if ( Well.IsFieldWinning( col, row, CurrentPlayer ) )
		{
			HandleWin();
		}
		else if ( Well.IsWellFull() )
		{
			HandleDraw();
		}

		if ( HasEnded )
		{
			GameEnded?.Invoke( this, (Hue)Winner );
		}
		else
		{
			UpdatePlayer();
		}

		TurnCompleted?.Invoke( this );
		return row;
	}

	/// <summary>
	/// sets <see cref="Winner"/> and calls <see cref="GameEnded"/> event
	/// </summary>
	[MemberNotNull( nameof( Winner ) )]
	private void HandleWin()
	{
		Winner = CurrentPlayer;
	}

	/// <summary>
	/// sets <see cref="Winner"/> to <see cref="Hue.None"/> and calls <see cref="GameEnded"/> event
	/// </summary>
	[MemberNotNull( nameof( Winner ) )]
	private void HandleDraw()
	{
		Winner = Hue.None;
	}

	/// <summary>
	/// updates <see cref="CurrentPlayer"/> and calls <see cref="PlayerSwitched"/> event
	/// </summary>
	private void UpdatePlayer()
	{
		Hue old = CurrentPlayer;
		CurrentPlayer = NextPlayer;

		PlayerSwitched?.Invoke( this, old, CurrentPlayer );
	}

	/// <summary>
	/// returns a copy of current well using <see cref="Well.Clone"/>
	/// </summary>
	/// <returns>copy of well</returns>
	public Well CloneWell()
	{
		return Well.Clone();
	}

}
