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

	HttpClient Http { get; } = new() { BaseAddress = new( "https://localhost:7126" ) };
	async Task<bool> CheckExists()
	{
		var resp = await Http.GetAsync( $"/api/multiplayer/{SelectedUuid}" ).ConfigureAwait( false );
		return resp.IsSuccessStatusCode;
	}
	async Task<Guid> CreateNew()
	{
		var createGameResp = await Http.PostAsync( "api/multiplayer", null ).ConfigureAwait( false );
		return await createGameResp.Content.ReadFromJsonAsync<Guid>().ConfigureAwait( false );
	}

}
