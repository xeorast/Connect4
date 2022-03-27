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
	public Game Game { get; set; }
	/// <summary>
	/// reference to tokens in grid stored in useful form
	/// </summary>
	Token[,] Tokens { get; set; }
	Button[] ColumnButtons { get; set; }

	public const int columns = 7;
	public const int rows = 6;

	Hue startingPlayer = Hue.Red;

	public ConnectFourGame()
	{
		InitializeComponent();

		InitializeGame();
	}

	[MemberNotNull( nameof( Game ) )]
	[MemberNotNull( nameof( Tokens ) )]
	[MemberNotNull( nameof( ColumnButtons ) )]
	void InitializeGame()
	{
		HideWinner();

		Game = new( width: columns, height: rows, starting: startingPlayer );
		startingPlayer = startingPlayer.Next( 2 );

		Game.PlayerMoved += Game_PlayerMoved;
		Game.GameEnded += Game_GameEnded;
		Game.ColumnFilled += Game_ColumnFilled;

		GameSwitched?.Invoke( Game );

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
		ShowWinner( winner );
	}

	/// <summary>
	/// disables column button
	/// </summary>
	/// <inheritdoc cref="Game.ColumnFilledEventHandler"/>
	private void Game_ColumnFilled( Game sender, int col )
	{
		ColumnButtons[col].IsEnabled = false;
	}

	/// <summary>
	/// restarts game when end screen is clicked
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void EndScreen_Click( object sender, RoutedEventArgs e )
	{
		InitializeGame();
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

}
