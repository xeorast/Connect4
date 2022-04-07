using Connect4.Domain.Core;
using System.ComponentModel.DataAnnotations;

namespace Connect4.Domain.Dtos;

public record BotRequestDto(
	[Required]
		WellDto Well,
	[Required]
	[Range( 1, 5 )]
		int Difficulty,
	[Required]
	[Range(1, int.MaxValue )]
		Hue CurrentPlayer );
