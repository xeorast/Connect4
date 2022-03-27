namespace Connect4.Engine;

/// <summary>
/// colors of players
/// </summary>
public enum Hue // because "color" had the same abbreviation as "column"
{
	None = 0,
	Red = 1,
	Yellow = 2,
	//Blue = 3,
}

public static class HueExtension
{
	static readonly Hue[] hues = Enum.GetValues<Hue>().Where( h => h != Hue.None ).ToArray();
	static readonly Dictionary<Hue, int> hueIx = hues.Select( ( x, i ) => (hue: x, index: i) ).ToDictionary( x => x.hue, x => x.index );

	/// <summary>
	/// gets color of next player given the limit of players
	/// </summary>
	/// <param name="hue">current player</param>
	/// <param name="limit">limit of players</param>
	/// <returns>next player</returns>
	/// <exception cref="ArgumentException">given hue does not belong to specified limit</exception>
	public static Hue Next( this Hue hue, int limit )
	{
		int curr = hueIx[hue];
		if ( curr > limit - 1 )
		{
			throw new ArgumentException( "given hue is beyond requested scope", nameof( limit ) );
		}
		if ( curr == limit - 1 )
		{
			return hues[0];
		}

		return hues[curr + 1];
	}
}