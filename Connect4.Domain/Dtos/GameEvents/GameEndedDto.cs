using Connect4.Engine;

namespace Connect4.Domain.Dtos.GameEvents;

public record GameEndedDto(
	Hue Winner );
