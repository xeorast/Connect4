using Connect4.Domain.Core;

namespace Connect4.Domain.Dtos.GameEvents;

public record GameEndedDto(
	Hue Winner );
