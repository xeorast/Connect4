using Connect4.Engine;
using Connect4.Engine.JsonConverters;
using System.Text.Json.Serialization;

namespace Connect4.Domain.Dtos;

public record WellDto(
	int ToConnect,
	Hue[,] Well )
{
	[JsonConverter( typeof( Array2DJsonConverter<Hue> ) )]
	public Hue[,] Well { get; init; } = Well;
};

