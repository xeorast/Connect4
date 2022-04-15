using Connect4.Api.Client;
using Connect4.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Connect4;
/// <summary>
/// Interaction logic for OnlineConnectionWindow.xaml
/// </summary>
public partial class OnlineConnectionWindow : Window
{
	public static RoutedUICommand ConnectCmd { get; } = new RoutedUICommand( "Connect", "ConnectCmd", typeof( OnlineConnectionWindow ) );
	public static RoutedUICommand CreateGameCmd { get; } = new RoutedUICommand( "CreateGame", "CreateGameCmd", typeof( OnlineConnectionWindow ) );

	public Hue SelectedPlayer { get; set; } = Hue.Red;
	public Guid? SelectedUuid
	{
		get => Guid.TryParse( UuidBox.Text, out var uuid ) ? uuid : null;
		private set => UuidBox.Text = value?.ToString() ?? string.Empty;
	}

	public OnlineConnectionWindow()
	{
		InitializeComponent();
	}

	private async void CreateGame_Executed( object sender, ExecutedRoutedEventArgs e )
	{
		var uuid = await CreateNew();
		SelectedUuid = uuid;
		Clipboard.SetText( uuid.ToString() );

		CommandManager.InvalidateRequerySuggested();
	}

	private void Connect_CanExecute( object sender, CanExecuteRoutedEventArgs e )
	{
		e.CanExecute = SelectedUuid is not null;
	}

	readonly C4ApiConsumer api = new( App.ApiPath );
	private async void Connect_Executed( object sender, ExecutedRoutedEventArgs e )
	{
		if ( SelectedUuid is null || !await CheckExists() )
		{
			UuidBox.Background = Brushes.Red;
			UuidBox.Foreground = Brushes.White;
		}
		else
		{
			DialogResult = true;
			Close();
		}
	}

	private void Close_Executed( object sender, ExecutedRoutedEventArgs e )
	{
		DialogResult = false;
		Close();
	}

	async Task<bool> CheckExists()
	{
		if ( SelectedUuid is Guid uuid )
		{
			var board = await api.Multiplayer.GetBoard( uuid );
			return board is not null;
		}
		return false;
	}
	async Task<Guid> CreateNew()
	{
		return await api.Multiplayer.CreateGame();
	}

}
