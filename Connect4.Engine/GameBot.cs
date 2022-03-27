namespace Connect4.Engine;

public class GameBot
{
	public Hue BotHue { get; }
	public int MaxLevel { get; }
	public int NumberPlayers { get; private set; }

	public GameBot( Hue botHue, int maxLevel, int numberPlayers )
	{
		BotHue = botHue;
		MaxLevel = maxLevel;
		NumberPlayers = numberPlayers;
	}

	//static readonly Random rng = new();
	public int GetRecommendation( Well well )
	{
		return Enumerable
			.Range( 0, well.Width )                                     // fot each column
			.Where( col => !well.IsColumnFull( col ) )              // which is not full
			.AsParallel()
			.WithDegreeOfParallelism( well.Width )                      // parallelise every column
			.Select( col =>
				(column: col,
				cost: CalculateCost( well, col, BotHue, MaxLevel )) )   // calculate cost of choosing it
			.OrderBy( option => option.cost )       // choose cheapest
			//.ThenBy( option => rng.Next() )							// in case of two equal take random
			.First()
			.column;                                                    // return choosen column
	}
	private float CalculateCost( Well well, int col, Hue startHue, int levesLeft )
	{
		// for propability calculate how many columns are available
		var validCols = Enumerable.Range( 0, well.Width )
			.Where( x => !well.IsColumnFull( x ) )
			.Count();

		// calculate if winnings are for bot or oponnnent
		float sign = startHue == BotHue ? -1.0f : 1.0f;

		// calculate cost of this shot at this exact level
		float cost = sign * CalculateCost( well, col, startHue ) / validCols;

		// if got back to bot, decrease level
		Hue nextHue = startHue.Next( NumberPlayers );
		int nextLevels = nextHue == BotHue ? levesLeft - 1 : levesLeft;

		// if game ends or limit of reached, do not check cost of following moves
		if ( cost != 0 || nextLevels <= 1 )
		{
			return cost;
		}

		// simulate shot
		var copy = well.Clone();
		_ = copy.InsertToken( col, startHue );

		//if ( levesLeft > 4 )
		//{
		//	_ = Parallel.For( 0, well.Width, i =>
		//	   {
		//		   if ( !copy.IsColumnFull( i ) )
		//		   {
		//			   cost += CalculateCost( copy, i, nextHue, nextLevels ) / 2;
		//		   }
		//	   } );
		//}
		//else
		//{

		// calculate cost of this shot
		for ( int i = 0; i < well.Width; i++ )
		{
			if ( !copy.IsColumnFull( i ) )
			{
				cost += CalculateCost( copy, i, nextHue, nextLevels ) / 2;
			}
		}
		//}

		return cost;
	}

	private static float CalculateCost( Well well, int col, Hue hue )
	{
		return well.IsFieldWinning( col, well.GetFreeRow( col ), hue ) ? 1.0f : 0.0f;
	}
}
