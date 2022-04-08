using Connect4.Domain.Core;
using Connect4.Domain.Core.GameWrappers;
using Connect4.Domain.Dtos.GameEvents;
using System.Windows;
using System.Windows.Input;

namespace Connect4;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	public static RoutedUICommand ExitCmd { get; } = new RoutedUICommand( "Exit", "ExitCmd", typeof( MainWindow ) );
	public static RoutedUICommand OptionsCmd { get; } = new RoutedUICommand( "Options", "OptionsCmd", typeof( MainWindow ) );

	public int RedWins
	{
		get => status.RedWins;
		set => status.RedWins = value;
	}
	public int YellowWins
	{
		get => status.YellowWins;
		set => status.YellowWins = value;
	}

	public MainWindow()
	{
		InitializeComponent();

		C4_GameSwitched( c4.GameWrapper );
		c4.GameSwitched += C4_GameSwitched;
	}

	/// <summary>
	/// subscribes to new game events
	/// </summary>
	/// <inheritdoc cref="ConnectFourGame.GameSwitchedHandler"/>
	private void C4_GameSwitched( GameWrapperBase game )
	{
		game.PlayerSwitched += Game_PlayerSwitched;
		game.GameEnded += Game_GameEnded;

		UpdatePlayerPresenter( game.CurrentPlayer );
	}

	/// <summary>
	/// sets color of current player mark
	/// </summary>
	/// <param name="hue"></param>
	void UpdatePlayerPresenter( Hue hue )
	{
		var key = ConnectFourGame.GetTokenClass( hue );
		status.playerPresenter.Style = Application.Current.Resources[key] as Style;
	}

	/// <summary>
	/// handler for switching player
	/// </summary>
	/// <inheritdoc cref="Game.PlayerSwitchedEventHandler"/>
	private void Game_PlayerSwitched( object? sender, PlayerSwitchedDto d )
	{
		Dispatcher.Invoke( () => UpdatePlayerPresenter( d.NewPlayer ) );
	}

	/// <summary>
	/// handler for game ending, increments player win count
	/// </summary>
	/// <inheritdoc cref="Game.GameEndedEventHandler"/>
	private void Game_GameEnded( object? sender, GameEndedDto d )
	{
		switch ( d.Winner )
		{
			case Hue.Red:
				++RedWins;
				break;

			case Hue.Yellow:
				++YellowWins;
				break;

			default:
				break;
		}
	}

	private void New_Command_Executed( object sender, System.Windows.Input.ExecutedRoutedEventArgs e )
	{
		c4.Restart();
	}

	private void Exit_Command_Executed( object sender, System.Windows.Input.ExecutedRoutedEventArgs e )
	{
		this.Close();
	}

	private void Options_Command_Executed( object sender, System.Windows.Input.ExecutedRoutedEventArgs e )
	{
		OptionsWindow options = new() { Owner = this, ShowInTaskbar = false };
		_ = options.ShowDialog();
	}
}
