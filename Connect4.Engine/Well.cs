using System.Text.Json;
using System.Text.Json.Serialization;

namespace Connect4.Engine;

/// <summary>
/// interface for json serialization
/// </summary>
public interface IWell
{
	int ToConnect { get; }
	[JsonConverter( typeof( JsonConverters.Array2DJsonConverter<Hue> ) )]
	Hue[,] WellObj { get; }
}

/// <summary>
/// extension for json serialization
/// </summary>
public static class WellExtensions
{
	public static string SerializeToJson( this IWell s, JsonSerializerOptions? options = null )=>
		JsonSerializer.Serialize( s, options );	
}

public class Well : IWell
{
	/// <summary>
	/// constructor for json deserialization
	/// </summary>
	/// <param name="toConnect"></param>
	/// <param name="wellObj"></param>
	[JsonConstructor]
	public Well( int toConnect, Hue[,] wellObj ) =>
		(Height, Width, ToConnect, WellObj) = (wellObj.GetLength( 1 ), wellObj.GetLength( 0 ), toConnect, wellObj);

	/// <summary>
	/// helper for json deserialization
	/// </summary>
	/// <param name="json"></param>
	/// <param name="options"></param>
	/// <returns></returns>
	public static Well? DeserializeFromJson( string json, JsonSerializerOptions? options = null ) =>
		JsonSerializer.Deserialize<Well>( json, options );

	public int Height { get; }
	public int Width { get; }
	private int TopRow => Height - 1;
	private int RightEnd => Width - 1;
	public int ToConnect { get; }
	/// <summary>
	/// 2D array of colors storing well fields states
	/// </summary>
	/// <remarks>
	/// ordered as [column, row]
	/// </remarks>
	[JsonConverter( typeof( JsonConverters.Array2DJsonConverter<Hue> ) )]
	public Hue[,] WellObj { get; set; }// col, row

	/// <summary>
	/// creates game with given parameters
	/// </summary>
	/// <param name="width">number of columns in well</param>
	/// <param name="height">number of rows in well</param>
	/// <param name="toConnect">numbers of token required for win</param>
	public Well( int width = 7, int height = 6, int toConnect = 4 )
	{
		Height = height;
		Width = width;
		ToConnect = toConnect;

		WellObj = new Hue[width, height];
	}

	/// <summary>
	/// creates game with given parameters
	/// </summary>
	/// <param name="well">2d array to base well on</param>
	/// <param name="toConnect">numbers of tokens required for win</param>
	public Well( Hue[,] well, int toConnect = 4 )
	{
		Width = well.GetLength( 0 );
		Height = well.GetLength( 1 );
		ToConnect = toConnect;

		WellObj = well;
	}

	/// <summary>
	/// inserts token into given column
	/// </summary>
	/// <param name="col">column to insert token to</param>
	/// <param name="hue">color of token to insert</param>
	/// <returns>row where token got inserted</returns>
	/// <exception cref="InvalidOperationException">thrown when attempting to insert token into full column</exception>
	public int InsertToken( int col, Hue hue )
	{
		// if top space is occupied, the move is invalid
		if ( IsColumnFull( col ) )
		{
			throw new InvalidOperationException( "collumn is ful" );
		}

		// fill lowest free space
		int row = GetFreeRow( col );

		WellObj[col, row] = hue;

		return row;
	}

	/// <summary>
	/// gets the nethermost not occupied row in given column
	/// </summary>
	/// <param name="col">the column</param>
	/// <returns>nethermost not occupied row in given column</returns>
	public int GetFreeRow( int col )
	{
		// place one above highest occupied space
		for ( int i = TopRow - 1; i >= 0; i-- )
		{
			if ( WellObj[col, i] != Hue.None )
			{
				return i + 1;
			}
		}

		return 0; // if no field is occupied, place at the bottom
	}

	/// <summary>
	/// checks if given field attends in winning streak of given color
	/// </summary>
	/// <param name="col">column</param>
	/// <param name="row">row</param>
	/// <param name="hue">the color</param>
	/// <returns>whether field attends in winning streak</returns>
	public bool IsFieldWinning( int col, int row, Hue hue )
	{
		// in each check add one in order to count both ends
		return MaxRow( col, row, hue ) - MinRow( col, row, hue ) + 1 >= ToConnect       // vertical span
			|| MaxCol( col, row, hue ) - MinCol( col, row, hue ) + 1 >= ToConnect       // horizontal span
			|| MaxMain( col, row, hue ) - MinMain( col, row, hue ) + 1 >= ToConnect     // main-diagonal span
			|| MaxAnti( col, row, hue ) - MinAnti( col, row, hue ) + 1 >= ToConnect;    // anti-diagonal span
	}

	#region winning streak checks
	/// <summary>
	/// maximum row attending in streak
	/// </summary>
	/// <param name="col"></param>
	/// <param name="row"></param>
	/// <param name="hue"></param>
	/// <returns></returns>
	private int MaxRow( int col, int row, Hue hue )
	{
		// top row must be maximum
		if ( row == TopRow )
		{
			return TopRow;
		}
		// find where this hue stops
		for ( int i = row + 1; i <= TopRow; i++ )
		{
			if ( WellObj[col, i] != hue )
			{
				return i - 1;
			}
		}
		// if didn't quit, then it's this hue all the way up
		return TopRow;
	}

	/// <summary>
	/// minimum row attending in streak
	/// </summary>
	/// <param name="col"></param>
	/// <param name="row"></param>
	/// <param name="hue"></param>
	/// <returns></returns>
	private int MinRow( int col, int row, Hue hue )
	{
		// bottom row must be minimum
		if ( row == 0 )
		{
			return row;
		}
		// find where this hue stops
		for ( int i = row - 1; i >= 0; i-- )
		{
			if ( WellObj[col, i] != hue )
			{
				return i + 1;
			}
		}
		// if didn't quit, then it's this hue all the way down
		return 0;
	}

	/// <summary>
	/// maximum column attending in streak
	/// </summary>
	/// <param name="col"></param>
	/// <param name="row"></param>
	/// <param name="hue"></param>
	/// <returns></returns>
	private int MaxCol( int col, int row, Hue hue )
	{
		// right column must be maximum
		if ( col == RightEnd )
		{
			return RightEnd;
		}
		// find where this hue stops
		for ( int i = col + 1; i <= RightEnd; i++ )
		{
			if ( WellObj[i, row] != hue )
			{
				return i - 1;
			}
		}
		// if didn't quit, then it's this hue all the way to the right
		return RightEnd;
	}

	/// <summary>
	/// minimum column attending in streak
	/// </summary>
	/// <param name="col"></param>
	/// <param name="row"></param>
	/// <param name="hue"></param>
	/// <returns></returns>
	private int MinCol( int col, int row, Hue hue )
	{
		// left column must be minimum
		if ( col == 0 )
		{
			return col;
		}
		// find where this hue stops
		for ( int i = col - 1; i >= 0; i-- )
		{
			if ( WellObj[i, row] != hue )
			{
				return i + 1;
			}
		}
		// if didn't quit, then it's this hue all the way to the left
		return 0;
	}

	/// <summary>
	/// maximum column attending in main diagonal streak
	/// </summary>
	/// <param name="col"></param>
	/// <param name="row"></param>
	/// <param name="hue"></param>
	/// <returns></returns>
	private int MaxMain( int col, int row, Hue hue )
	{
		// right/bottom edge must be maximum
		if ( col == RightEnd || row == 0 )
		{
			return col;
		}
		// find where this hue stops
		for ( int i = 1; i <= RightEnd - col && i <= row; i++ )
		{
			if ( WellObj[col + i, row - i] != hue )
			{
				return col + i - 1;
			}
		}
		// if didn't quit, then it's this hue all the way to the nearrest bottom/right edge
		return col + Math.Min( RightEnd - col, row );
	}

	/// <summary>
	/// minimum column attending in main diagonal streak
	/// </summary>
	/// <param name="col"></param>
	/// <param name="row"></param>
	/// <param name="hue"></param>
	/// <returns></returns>
	private int MinMain( int col, int row, Hue hue )
	{
		// left/top edge must be minimum
		if ( col == 0 || row == TopRow )
		{
			return col;
		}
		// find where this hue stops
		for ( int i = 1; i <= col && i <= TopRow - row; i++ )
		{
			if ( WellObj[col - i, row + i] != hue )
			{
				return col - i + 1;
			}
		}
		// if didn't quit, then it's this hue all the way to the nearrest top/left edge
		return col - Math.Min( col, TopRow - row );
	}

	/// <summary>
	/// maximum column attending in anti-diagonal streak
	/// </summary>
	/// <param name="col"></param>
	/// <param name="row"></param>
	/// <param name="hue"></param>
	/// <returns></returns>
	private int MaxAnti( int col, int row, Hue hue )
	{
		// right/top edge must be maximum
		if ( col == RightEnd || row == TopRow )
		{
			return col;
		}
		// find where this hue stops
		for ( int i = 1; i <= RightEnd - col && i <= TopRow - row; i++ )
		{
			if ( WellObj[col + i, row + i] != hue )
			{
				return col + i - 1;
			}
		}
		// if didn't quit, then it's this hue all the way to the nearrest top/right edge
		return col + Math.Min( RightEnd - col, TopRow - row );
	}

	/// <summary>
	/// minimum column attending in anti-diagonal streak
	/// </summary>
	/// <param name="col"></param>
	/// <param name="row"></param>
	/// <param name="hue"></param>
	/// <returns></returns>
	private int MinAnti( int col, int row, Hue hue )
	{
		// left/bottom edge must be minimum
		if ( col == 0 || row == 0 )
		{
			return col;
		}
		// find where this hue stops
		for ( int i = 1; i <= col && i <= row; i++ )
		{
			if ( WellObj[col - i, row - i] != hue )
			{
				return col - i + 1;
			}
		}
		// if didn't quit, then it's this hue all the way to the nearrest bottom/left edge
		return col - Math.Min( col, row );
	}

	#endregion

	/// <summary>
	/// gets <see cref="IEnumerable{T}"/> of all fields attending in winning streak
	/// </summary>
	/// <returns>fields attending in streak</returns>
	public IEnumerable<(int col, int row)> GetWinning()
	{
		return from col in Enumerable.Range( 0, Width )
			   from row in Enumerable.Range( 0, Height )
			   let hue = WellObj[col, row]
			   where hue != Hue.None && IsFieldWinning( col, row, hue )
			   select (col, row);
	}


	/// <summary>
	/// checks if given column is full
	/// </summary>
	/// <param name="col">column</param>
	/// <returns>whether column is full</returns>
	public bool IsColumnFull( int col )
	{
		return WellObj[col, TopRow] != Hue.None;
	}

	/// <summary>
	/// checks if well is full
	/// </summary>
	/// <returns>whether well is full</returns>
	public bool IsWellFull()
	{
		for ( int i = 0; i < Width; i++ )
		{
			if ( !IsColumnFull( i ) )
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// prints current state in the console
	/// </summary>
	public void Draw()
	{
		for ( int row = Height - 1; row >= 0; row-- )
		{
			for ( int col = 0; col < Width; col++ )
			{
				if ( WellObj[col, row] == Hue.None )
				{
					Console.Write( "- " );
				}
				else
				{
					Console.Write( $"{WellObj[col, row]:d} " );
				}
			}
			Console.WriteLine();
		}
		Console.WriteLine();
	}

	/// <summary>
	/// copying constructor
	/// </summary>
	/// <param name="base"><see cref="Well"/> on which to base on</param>
	private Well( Well @base )
	{
		Height = @base.Height;
		Width = @base.Width;
		ToConnect = @base.ToConnect;

		WellObj = (Hue[,])@base.WellObj.Clone();
	}

	/// <summary>
	/// returns a copy of current well using <see cref="Well(Well)"/>
	/// </summary>
	/// <returns>copy of well</returns>
	public Well Clone()
	{
		return new( this );
	}

}
