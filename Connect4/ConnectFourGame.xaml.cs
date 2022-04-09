using Connect4.Domain.Core;
using Connect4.Domain.Core.GameWrappers;
using Connect4.Domain.Dtos.GameEvents;
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

	public GameWrapperBase GameWrapper { get; set; }
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
	volatile bool hasTurnEnded;
	public TimeSpan MinBotMoveTime { get; set; }
	(Token token, Hue hue)? LastToken =>
		GameWrapper.PreviousMove is null ?
			null
			: (Tokens[GameWrapper.PreviousMove.Value.Column, GameWrapper.PreviousMove.Value.Row], GameWrapper[GameWrapper.PreviousMove.Value]);

	bool CanMove => GameWrapper is GameWrapperBase { IsNowPlayer: true, CanMove: true, HasEnded: false };

	public ConnectFourGame()
	{
		InitializeComponent();

		InitializeGame();
	}

	public void Restart()
	{
		InitializeGame();
	}

	[MemberNotNull( nameof( GameWrapper ) )]
	[MemberNotNull( nameof( Tokens ) )]
	[MemberNotNull( nameof( ColumnButtons ) )]
	[MemberNotNull( nameof( GameTimer ) )]
	void InitializeGame()
	{
		HideWinner();

		// game
		//GameWrapper = new LocalGameWrapper( columns, rows, StartingPlayer, (GameWrapperBase.GameMode)UserSettings.Default.GameMode );//todo: use one type
		OnlineGameWrapper onlineGameWrapper = new( Hue.Red );
		onlineGameWrapper.Connect(/* Guid.Parse( "6cc7ed1c-c3ae-4972-88cd-97f154ea27e1" ) */).GetAwaiter().GetResult();
		GameWrapper = onlineGameWrapper;
		StartingPlayer = StartingPlayer.Next( 2 );
		GameWrapper.PlayerMoved += Game_PlayerMoved;
		GameWrapper.GameEnded += Game_GameEnded;
		GameWrapper.ColumnFilled += Game_ColumnFilled;
		GameWrapper.TurnCompleted += Game_TurnCompleted;

		GameSwitched?.Invoke( GameWrapper );

		MinBotMoveTime = new( 0, 0, 0, 0, UserSettings.Default.MinBotMoveTime );
		InitializeGrid();
		InitializeColumnButtons();
		InitializeTimer();

		bool enable = CanMove;
		foreach ( var button in ColumnButtons )
		{
			Dispatcher.Invoke( () => { button.IsEnabled = enable; } );
		}
	}

	[MemberNotNull( nameof( Tokens ) )]
	private void InitializeGrid()
	{
		Tokens = new Token[GameWrapper.Columns, GameWrapper.Rows];

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
		hasTurnEnded = true;
		GameTimer.Start();
	}


	/// <summary>
	/// handler for <see cref="GameSwitched"/> event
	/// </summary>
	/// <param name="game">new game instance</param>
	public delegate void GameSwitchedHandler( GameWrapperBase game );
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
		if ( LastToken is not null )
		{
			var lastKey = GetTokenClass( LastToken.Value.hue );
			LastToken.Value.token.Style = Application.Current.Resources[lastKey] as Style;
		}

		var key = GetNewTokenClass( hue );
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
		if ( GameWrapper.Winner is Hue.None )
		{
			return;
		}

		foreach ( Coordinate cord in GameWrapper.GetWinning() )
		{
			var hue = GameWrapper[cord];

			var key = GetNegativeTokenClass( hue );
			Tokens[cord.Column, cord.Row].Style = Application.Current.Resources[key] as Style;
		}
	}

	/// <summary>
	/// performs move based on bot decision
	/// </summary>
	/// <returns></returns>
	void MoveBot()
	{
		//if ( Game.CurrentPlayer != Hue.Yellow )
		//{
		//	throw new Exception( "error" );
		//}
		GameWrapper.MoveBot( MinBotMoveTime );
		//if ( Game.CurrentPlayer != Hue.Yellow )
		//{
		//	throw new Exception( "worse error" );
		//}
	}

	readonly SemaphoreSlim semaphore = new( 1, 1 );
	/// <summary>
	/// invoked every <see cref="timerInterval"/> and moves bot if its needed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Timer_Elapsed( object? sender, System.Timers.ElapsedEventArgs e )
	{
		if ( !semaphore.Wait( (int)timerInterval ) )
		{
			return;
		}
		if ( hasTurnEnded && !GameWrapper.HasEnded )
		{
			hasTurnEnded = false;
			if ( GameWrapper.IsNowBot )
			{
				MoveBot();
			}
		}
		_ = semaphore.Release();
	}

	/// <summary>
	/// performs move
	/// </summary>
	/// <param name="sender">button with <see cref="System.Windows.FrameworkElement.Tag"/> set to column index</param>
	/// <param name="e"></param>
	private void Column_Click( object sender, RoutedEventArgs e )
	{
		if ( CanMove )
		{
			var btn = (Button)sender;
			var col = (int)btn.Tag;
			GameWrapper.Move( col );
		}

	}

	/// <summary>
	/// displays token
	/// </summary>
	/// <inheritdoc cref="Game.PlayerMovedEventHandler"/>
	private void Game_PlayerMoved( object? sender, PlayerMovedDto d )
	{
		Dispatcher.Invoke( () => ShowToken( d.Column, d.Row, d.Player ) );
	}

	/// <summary>
	/// shows winner when game ends
	/// </summary>
	/// <inheritdoc cref="Game.GameEndedEventHandler"/>
	private void Game_GameEnded( object? sender, GameEndedDto d )
	{
		GameTimer?.Stop();
		Dispatcher.Invoke( () => ShowWinner( d.Winner ) );
		Dispatcher.Invoke( () => ColorWinning() );
	}

	/// <summary>
	/// disables column button
	/// </summary>
	/// <inheritdoc cref="Game.ColumnFilledEventHandler"/>
	private void Game_ColumnFilled( object? sender, ColumnFilledDto d )
	{
		Dispatcher.Invoke( () => { ColumnButtons[d.Column].IsEnabled = false; } );
	}

	private void Game_TurnCompleted( object? sender, EventArgs e )
	{
		bool enable = CanMove;
		foreach ( var button in ColumnButtons )
		{
			Dispatcher.Invoke( () => { button.IsEnabled = enable; } );//todo: this overrides column being full
		}
		hasTurnEnded = true;
	}


	/// <summary>
	/// restarts game when end screen is clicked
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void EndScreen_Click( object? sender, RoutedEventArgs e )
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
	/// gets name of token style for newly inserted token
	/// </summary>
	/// <param name="hue">color</param>
	/// <returns></returns>
	public static string GetNewTokenClass( Hue hue )
	{
		return hue switch
		{
			Hue.Yellow => "YellowTokenDark",
			Hue.Red => "RedTokenDark",
			_ => "PinkTokenDark",
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
