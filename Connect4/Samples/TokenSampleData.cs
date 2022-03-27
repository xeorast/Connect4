//<local:Token xmlns:local="clr-namespace:Connect4" 
//		 xmlns="http://schemas.microsoft.com/netfx/2009/xaml/presentation" FrontColor="#FFA52FA5" BackColor="#FFE264E2" Tag="hello">
//	<local:Token.FrameColor>
//		<RadialGradientBrush>
//			<GradientStop Color="#FF4B4DEC" Offset="0.9"/>
//			<GradientStop Color="#FF4B4CD8" Offset="1"/>
//		</RadialGradientBrush>
//	</local:Token.FrameColor>

//</local:Token>

using System.Windows.Media;

namespace Connect4.Samples;

public static class TokenSampleData
{
	public static Token TokenSample
	{
		get
		{
			return new Token()
			{
				FrontColor = new SolidColorBrush( (Color)ColorConverter.ConvertFromString( "#FFA52FA5" ) ),
				BackColor = new SolidColorBrush( (Color)ColorConverter.ConvertFromString( "#FFE264E2" ) )
			};
		}
	}
}
