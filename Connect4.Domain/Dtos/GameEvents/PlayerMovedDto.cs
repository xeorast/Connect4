using Connect4.Domain.Core;

namespace Connect4.Domain.Dtos.GameEvents;

public record PlayerMovedDto(
	int Column,
	int Row,
	Hue Player );
