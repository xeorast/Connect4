using Connect4.Engine;
using System.Windows;

namespace Connect4;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
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

		C4_GameSwitched( c4.Game );
		c4.GameSwitched += C4_GameSwitched;
	}

	/// <summary>
	/// subscribes to new game events
	/// </summary>
	/// <inheritdoc cref="ConnectFourGame.GameSwitchedHandler"/>
	private void C4_GameSwitched( Game game )
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
	private void Game_PlayerSwitched( Game sender, Hue oldPlayer, Hue newPlayer )
	{
		UpdatePlayerPresenter( newPlayer );
	}

	/// <summary>
	/// handler for game ending, increments player win count
	/// </summary>
	/// <inheritdoc cref="Game.GameEndedEventHandler"/>
	private void Game_GameEnded( Game sender, Hue winner )
	{
		switch ( winner )
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

}
