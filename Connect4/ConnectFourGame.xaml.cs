using Connect4.Engine;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace Connect4;

/// <summary>
/// Interaction logic for ConnectFourGame.xaml
/// </summary>
public partial class ConnectFourGame : UserControl
{
	public const int columns = 7;
	public const int rows = 6;
	public const double timerInterval = 50;

	public Game Game { get; set; }
	/// <summary>
	/// reference to tokens in grid stored in useful form
	/// </summary>
	Token[,] Tokens { get; set; }
	Button[] ColumnButtons { get; set; }
	Hue StartingPlayer { get; set; } = Hue.Red;

	/// <summary>
	/// game timer for sort of main loop
	/// </summary>
	System.Timers.Timer GameTimer { get; set; }
	bool HasTurnEnded { get; set; }
	public int MinBotMoveTime { get; set; }


	Dictionary<Hue, GameBot> Bots { get; set; }
	Dictionary<Hue, PlayerType> Players { get; set; } = new();
	enum PlayerType
	{
		Player,
		Computer
	}

	public ConnectFourGame()
	{
		InitializeComponent();

		InitializeGame();
	}

	public void Restart()
	{
		InitializeGame();
	}

	[MemberNotNull( nameof( Game ) )]
	[MemberNotNull( nameof( Tokens ) )]
	[MemberNotNull( nameof( ColumnButtons ) )]
	[MemberNotNull( nameof( Bots ) )]
	[MemberNotNull( nameof( GameTimer ) )]
	void InitializeGame()
	{
		HideWinner();

		// game
		Game = new( width: columns, height: rows, starting: StartingPlayer );
		StartingPlayer = StartingPlayer.Next( 2 );

		Game.PlayerMoved += Game_PlayerMoved;
		Game.GameEnded += Game_GameEnded;
		Game.ColumnFilled += Game_ColumnFilled;
		Game.TurnCompleted += Game_TurnCompleted;

		GameSwitched?.Invoke( Game );

		InitializeBots();
		InitializeGrid();
		InitializeColumnButtons();
		InitializeTimer();
	}

	[MemberNotNull( nameof( Bots ) )]
	private void InitializeBots()
	{
		var level = Math.Clamp( UserSettings.Default.Difficulty, 1, 5 );
		Bots = Enum.GetValues<Hue>()
			.ToDictionary( hue => hue, hue => new GameBot( hue, level, 2 ) );

		var player1 = UserSettings.Default.GameMode switch
		{
			GameMode.CvC => PlayerType.Computer,
			_ => PlayerType.Player
		};
		var player2 = UserSettings.Default.GameMode switch
		{
			GameMode.PvP => PlayerType.Player,
			_ => PlayerType.Computer
		};

		Players[Hue.Red] = player1;
		Players[Hue.Yellow] = player2;

		MinBotMoveTime = UserSettings.Default.MinBotMoveTime;
	}

	[MemberNotNull( nameof( Tokens ) )]
	private void InitializeGrid()
	{
		Tokens = new Token[Game.Width, Game.Height];

		gameGrid.Children.Clear();
		gameGrid.Columns = columns;
		gameGrid.Rows = rows;

		var tokenStyle = Application.Current.Resources["HiddenToken"] as Style;

		for ( int row = rows - 1; row >= 0; row-- )
		{
			for ( int col = 0; col < columns; col++ )
			{
				Token token = new() { Style = tokenStyle };
				Tokens[col, row] = token;
				_ = gameGrid.Children.Add( token );
			}
		}
	}

	[MemberNotNull( nameof( ColumnButtons ) )]
	private void InitializeColumnButtons()
	{
		ColumnButtons = new Button[columns];
		hitTestGrid.Children.Clear();
		hitTestGrid.Columns = columns;

		Style? buttonStyle = Application.Current.Resources["ColumnButton"] as Style;

		for ( int col = 0; col < columns; col++ )
		{
			Button button = new() { Tag = col, Style = buttonStyle };
			button.Click += Column_Click;
			ColumnButtons[col] = button;
			_ = hitTestGrid.Children.Add( button );
		}
	}

	[MemberNotNull( nameof( GameTimer ) )]
	private void InitializeTimer()
	{
		GameTimer = new( timerInterval );
		GameTimer.Elapsed += Timer_Elapsed;
		HasTurnEnded = true;
		GameTimer.Start();
	}


	/// <summary>
	/// handler for <see cref="GameSwitched"/> event
	/// </summary>
	/// <param name="game">new game instance</param>
	public delegate void GameSwitchedHandler( Game game );
	/// <summary>
	/// called when new game is initialized
	/// </summary>
	public event GameSwitchedHandler? GameSwitched;

	/// <summary>
	/// sets token style to given color
	/// </summary>
	/// <param name="col">token column</param>
	/// <param name="row">token row</param>
	/// <param name="hue">color</param>
	void ShowToken( int col, int row, Hue hue )
	{
		var key = GetTokenClass( hue );

		Tokens[col, row].Style = Application.Current.Resources[key] as Style;
	}

	/// <summary>
	/// shows winner screen
	/// </summary>
	/// <param name="winner">winner color</param>
	void ShowWinner( Hue winner )
	{
		( (UIElement)resultBadge.Parent ).Visibility = Visibility.Visible;
		resultBadge.Text = winner switch
		{
			Hue.Red => "Red wins!",
			Hue.Yellow => "Yellow wins!",
			Hue.None => "That' a draw!",
			_ => "ooops! error"
		};
	}

	/// <summary>
	/// hides winner screen
	/// </summary>
	void HideWinner()
	{
		( (UIElement)resultBadge.Parent ).Visibility = Visibility.Collapsed;
	}

	/// <summary>
	/// colors tokens attending in winning streak
	/// </summary>
	void ColorWinning()
	{
		if ( Game.Winner is Hue.None )
		{
			return;
		}

		var well = Game.CloneWell();
		foreach ( (int col, int row) in well.GetWinning() )
		{
			var hue = well.WellObj[col, row];

			var key = GetNegativeTokenClass( hue );
			Tokens[col, row].Style = Application.Current.Resources[key] as Style;
		}
	}

	/// <summary>
	/// performs move based on bot decision
	/// </summary>
	/// <returns></returns>
	async Task MoveBot()
	{
		var delay = Task.Delay( MinBotMoveTime );
		var col = Bots[Game.CurrentPlayer].GetRecommendation( Game.CloneWell() );
		await delay;

		Dispatcher.Invoke( () => { _ = Game.Move( col ); } );
	}


	/// <summary>
	/// invoked every <see cref="timerInterval"/> and moves bot if its needed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private async void Timer_Elapsed( object? sender, System.Timers.ElapsedEventArgs e )
	{
		if ( HasTurnEnded && !Game.HasEnded )
		{
			HasTurnEnded=false;
			if ( Players[Game.CurrentPlayer] is PlayerType.Computer )
			{
				await MoveBot();
			}
		}
	}

	/// <summary>
	/// performs move
	/// </summary>
	/// <param name="sender">button with <see cref="System.Windows.FrameworkElement.Tag"/> set to column index</param>
	/// <param name="e"></param>
	private void Column_Click( object sender, RoutedEventArgs e )
	{
		var btn = (Button)sender;
		var col = (int)btn.Tag;

		_ = Game.Move( col );

	}

	/// <summary>
	/// displays token
	/// </summary>
	/// <inheritdoc cref="Game.PlayerMovedEventHandler"/>
	private void Game_PlayerMoved( Game sender, int col, int row, Hue hue )
	{
		ShowToken( col, row, hue );
	}

	/// <summary>
	/// shows winner when game ends
	/// </summary>
	/// <inheritdoc cref="Game.GameEndedEventHandler"/>
	private void Game_GameEnded( Game sender, Hue winner )
	{
		GameTimer?.Stop();
		ShowWinner( winner );
		ColorWinning();
	}

	/// <summary>
	/// disables column button
	/// </summary>
	/// <inheritdoc cref="Game.ColumnFilledEventHandler"/>
	private void Game_ColumnFilled( Game sender, int col )
	{
		ColumnButtons[col].IsEnabled = false;
	}

	private void Game_TurnCompleted( Game sender )
	{
		bool disable = Players[Game.CurrentPlayer] is PlayerType.Computer;
		foreach ( var button in ColumnButtons )
		{
			Dispatcher.Invoke( () => { button.IsEnabled = !disable; } );
		}
		HasTurnEnded = true;
	}


	/// <summary>
	/// restarts game when end screen is clicked
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void EndScreen_Click( object sender, RoutedEventArgs e )
	{
		Restart();
	}

	/// <summary>
	/// gets name of token style corresponding to given color
	/// </summary>
	/// <param name="hue">color</param>
	/// <returns></returns>
	public static string GetTokenClass( Hue hue )
	{
		return hue switch
		{
			Hue.Yellow => "YellowToken",
			Hue.Red => "RedToken",
			_ => "PinkToken",
		};
	}

	/// <summary>
	/// gets name of inversed token style corresponding to given color
	/// </summary>
	/// <param name="hue">color</param>
	/// <returns></returns>
	public static string GetNegativeTokenClass( Hue hue )
	{
		return hue switch
		{
			Hue.Yellow => "YellowTokenInversed",
			Hue.Red => "RedTokenInversed",
			_ => "PinkTokenInversed",
		};
	}

}
