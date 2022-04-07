using System.Text.Json;
using System.Text.Json.Serialization;

namespace Connect4.Domain.JsonConverters;

public class Array2DJsonConverter<T> : JsonConverter<T[,]> where T : struct, Enum
{
	public static JsonSerializerOptions OptionsWithConverter => optionsWithConverter;
	private readonly static JsonSerializerOptions optionsWithConverter = new()
	{
		Converters = { new Array2DJsonConverter<T>() }
	};

	public static bool TryReadToken( ref Utf8JsonReader reader, JsonTokenType tokenType )
	{
		if ( reader.TokenType != tokenType )
		{
			return false;
		}
		return reader.Read();
	}

	public override T[,] Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
	{
		// throw if wrong type
		if ( typeToConvert != typeof( T[,] ) )
		{
			throw new JsonException( $"converter of type {typeof( Array2DJsonConverter<T> ).Name} can only parse {typeof( T ).Name}[][]" );
		}

		// throw if not array
		if ( !TryReadToken( ref reader, JsonTokenType.StartArray ) )
		{
			throw new JsonException( $"value other than array passed to {typeof( Array2DJsonConverter<T> ).Name}" );
		}

		List<List<T>> tmp = new();

		// while array not closed
		while ( reader.TokenType != JsonTokenType.EndArray )
		{
			// enter if new record
			if ( TryReadToken( ref reader, JsonTokenType.StartArray ) )
			{
				List<T> local = new();
				// while not end of record
				while ( !TryReadToken( ref reader, JsonTokenType.EndArray ) )
				{
					// read as number
					if ( reader.TokenType == JsonTokenType.Number )
					{
						local.Add( Enum.Parse<T>( reader.GetInt32().ToString() ) );
						_ = reader.Read();
					}
					// or read as string
					else if ( reader.TokenType == JsonTokenType.String )
					{
						local.Add( Enum.Parse<T>( reader.GetString()! ) );
						_ = reader.Read();
					}
					// or ignore comment
					else if ( options.ReadCommentHandling == JsonCommentHandling.Allow
						&& reader.TokenType == JsonTokenType.Comment )
					{
						_ = reader.Read();
					}
					// or throw if unexpected type
					else
					{
						throw new JsonException( "unknown character found while reading 2d array" );
					}

				}
				// append record
				tmp.Add( local );
			}
		}

		// final array sizes
		var fstLen = tmp.Count;
		var scndLen = tmp.Count > 0 ? tmp[0].Count : 0;

		// throw if 2nd dimension arrays are not the same size
		if ( tmp.Any( x => x.Count != scndLen ) )
		{
			throw new JsonException( "2d array does not have constant number of items in 2nd dimension" );
		}

		// final array
		T[,] ret = new T[fstLen, scndLen];

		// fill array
		for ( int col = 0; col < fstLen; col++ )
		{
			for ( int row = 0; row < scndLen; row++ )
			{
				ret[col, row] = tmp[col][row];
			}
		}

		return ret;
	}

	public override void Write( Utf8JsonWriter writer, T[,] value, JsonSerializerOptions options )
	{
		// convert T[,] to T[][]
		T[][] arr = value
			.Cast<T>()
			.Select( ( x, i ) => (value: x, index: i) )        // get index
			.GroupBy(
			   x => x.index / value.GetLength( 1 ), // put into groups of <width> items 
			   x => x.value )
			.Select( group => group.ToArray() )       // make each column into separate array
			.ToArray();                                             // put those into array of arrays

		// begin outer array
		writer.WriteStartArray();
		foreach ( T[] column in arr )
		{
			// begin inner array
			writer.WriteStartArray();

			// write all records
			foreach ( T item in column )
			{
				writer.WriteStringValue( item.ToString( "d" ) );
			}

			writer.WriteEndArray();
		}
		writer.WriteEndArray();
	}
}
