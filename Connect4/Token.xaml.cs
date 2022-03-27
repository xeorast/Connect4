using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Connect4;

/// <summary>
/// Interaction logic for Token.xaml
/// </summary>
public partial class Token : UserControl
{
	/// <summary>
	/// token frame and letters
	/// </summary>
	public Brush? FrontColor
	{
		get { return (Brush?)GetValue( FrontColorProperty ); }
		set { SetValue( FrontColorProperty, value ); }
	}

	// Using a DependencyProperty as the backing store for FrontColor.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty FrontColorProperty =
		DependencyProperty.Register( "FrontColor", typeof( Brush ), typeof( Token ), new PropertyMetadata( Brushes.DarkTurquoise ) );

	/// <summary>
	/// token background
	/// </summary>
	public Brush? BackColor
	{
		get { return (Brush?)GetValue( BackColorProperty ); }
		set { SetValue( BackColorProperty, value ); }
	}

	// Using a DependencyProperty as the backing store for BackColor.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty BackColorProperty =
		DependencyProperty.Register( "BackColor", typeof( Brush ), typeof( Token ), new PropertyMetadata( Brushes.Bisque ) );

	public Token()
	{
		InitializeComponent();
	}
}
