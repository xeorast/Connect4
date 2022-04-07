using Connect4.Domain.Core;

namespace Connect4.Domain.Dtos;

public record GameDto(
		int NumberPlayers,
		Hue? Winner,
		Hue CurrentPlayer,
		WellDto Well );
