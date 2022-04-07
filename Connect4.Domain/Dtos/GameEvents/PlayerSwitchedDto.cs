using Connect4.Domain.Core;

namespace Connect4.Domain.Dtos.GameEvents;

public record PlayerSwitchedDto(
	Hue OldPlayer,
	Hue NewPlayer );
