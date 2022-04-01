using Connect4.Api.JsonConverters;
using Connect4.Engine;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Connect4.Api.Controllers;

[ApiController]
[Route( "api/[controller]" )]
public class BotController : ControllerBase
{
	[HttpPost]
	public ActionResult<int> Get( BotRequestDto req )
	{
		GameBot bot = new( req.CurrentPlayer, req.Difficulty, 2 );

		Well well = new( req.Well.Well, req.Well.ToConnect );

		if ( well.IsWellFull() )
		{
			return BadRequest( "cannot generate suggestion for full well" );
		}

		return bot.GetRecommendation( well );
	}

	public record WellDto(
		int ToConnect,
		Hue[,] Well )
	{
		[JsonConverter( typeof( Array2DJsonConverter<Hue> ) )]
		public Hue[,] Well { get; init; } = Well;
	};

	public record BotRequestDto(
		[Required]
			WellDto Well,
		[Required]
		[Range( 1, 5 )]
			int Difficulty,
		[Required]
		[Range(1, int.MaxValue )]
			Hue CurrentPlayer );
}
