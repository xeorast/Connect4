using Connect4.Engine;

namespace Connect4.Domain.Dtos;

public record GameDto(
		int NumberPlayers,
		Hue? Winner,
		Hue CurrentPlayer,
		WellDto Well );
