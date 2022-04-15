using System.Windows;

namespace Connect4;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
#if DEBUG
	public static readonly Uri ApiPath = new( "https://localhost:7126" );
#else
	public static readonly Uri ApiPath = new( "https://xeo-connect-4.herokuapp.com/" );
#endif

}
