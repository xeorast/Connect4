using System.Windows.Controls;

namespace Connect4;

/// <summary>
/// Interaction logic for StatusBar.xaml
/// </summary>
public partial class StatusBar : UserControl
{
	/// <summary>
	/// int version of red wins counter
	/// </summary>
	public int RedWins
	{
		get => int.Parse( redWinDisplay.Text );
		set => redWinDisplay.Text = value.ToString();
	}
	/// <summary>
	/// int version of yellow wins counter
	/// </summary>
	public int YellowWins
	{
		get => int.Parse( yellowWinDisplay.Text );
		set => yellowWinDisplay.Text = value.ToString();
	}

	public StatusBar()
	{
		InitializeComponent();
	}
}
