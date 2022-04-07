using Connect4.Domain.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Connect4.Domain.JsonConverters;

public class IWellJsonConverter : JsonConverter<IWell>
{
	public override IWell? Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
	{
		return JsonSerializer.Deserialize<Well>( ref reader, options );
	}

	public override void Write( Utf8JsonWriter writer, IWell value, JsonSerializerOptions options )
	{
		JsonSerializer.Serialize( writer, value, options );
	}
}
