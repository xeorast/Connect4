using Connect4.Engine;

namespace Connect4.Domain.Dtos.GameEvents;

public record PlayerMovedDto(
	int Column,
	int Row,
	Hue Player );
