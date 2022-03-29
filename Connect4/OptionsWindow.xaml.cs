using System.Windows;
using System.Windows.Input;

namespace Connect4;

public enum GameMode
{
	PvP,
	PvC,
	CvC
}

/// <summary>
/// Interaction logic for OptionsWindow.xaml
/// </summary>
public partial class OptionsWindow : Window
{
	private GameMode SavedGameMode { get; set; }
	public GameMode GameMode
	{
		get => UserSettings.Default.GameMode;
		set { UserSettings.Default.GameMode = value; UpdateVisibilities(); }
	}

	private int SavedDifficulty { get; set; }
	public int Difficulty
	{
		get => UserSettings.Default.Difficulty;
		set { UserSettings.Default.Difficulty = value; UpdateVisibilities(); }
	}
	private int SavedMinBotMoveTime { get; set; }
	public int MinBotMoveTime
	{
		get => UserSettings.Default.MinBotMoveTime;
		set { UserSettings.Default.MinBotMoveTime = value; UpdateVisibilities(); }
	}

	public bool WasModified => 
		GameMode != SavedGameMode
		|| Difficulty != SavedDifficulty
		|| MinBotMoveTime != SavedMinBotMoveTime;

	void UpdateVisibilities()
	{
		UpdateSavingNoteVisibility();
		UpdateBotPropertiesVisibility();
	}
	void UpdateSavingNoteVisibility() => SavingNoteVisibility = WasModified ? Visibility.Visible : Visibility.Collapsed;
	void UpdateBotPropertiesVisibility() => BotPropertiesVisibility = GameMode is GameMode.PvC or GameMode.CvC;

	public Visibility SavingNoteVisibility
	{
		get { return (Visibility)GetValue( SavingNoteVisibbilityProperty ); }
		set { SetValue( SavingNoteVisibbilityProperty, value ); }
	}
	public static readonly DependencyProperty SavingNoteVisibbilityProperty =
		DependencyProperty.Register( "SavingNoteVisibility", typeof( Visibility ), typeof( OptionsWindow ), new PropertyMetadata( Visibility.Hidden ) );

	public bool BotPropertiesVisibility
	{
		get { return (bool)GetValue( BotPropertiesVisibilityProperty ); }
		set { SetValue( BotPropertiesVisibilityProperty, value ); }
	}
	public static readonly DependencyProperty BotPropertiesVisibilityProperty =
		DependencyProperty.Register( "BotPropertiesVisibility", typeof( bool ), typeof( OptionsWindow ), new PropertyMetadata( true ) );


	void UpdateCopy()
	{
		SavedGameMode = GameMode;
		SavedDifficulty = Difficulty;
		SavedMinBotMoveTime = MinBotMoveTime;
	}

	public OptionsWindow()
	{
		InitializeComponent();

		UpdateCopy();

		UpdateVisibilities();
	}

	private void Save_Command_Executed( object sender, ExecutedRoutedEventArgs e )
	{
		UserSettings.Default.Save();

		UpdateCopy();

		Close();
	}

	private void Close_Command_Executed( object sender, ExecutedRoutedEventArgs e )
	{
		UserSettings.Default.Reload();
		Close();
	}

	private void Window_Closing( object sender, System.ComponentModel.CancelEventArgs e )
	{
		if ( WasModified )
		{
			var result = MessageBox.Show( "Unsaved changes. Continue?", "", MessageBoxButton.OKCancel );
			if ( result is MessageBoxResult.Cancel )
			{
				e.Cancel = true;
			}
		}
	}
}
