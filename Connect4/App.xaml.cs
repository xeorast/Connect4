using System.Windows;

namespace Connect4;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	public static readonly Uri ApiPath = new( "https://localhost:7126" );
}
