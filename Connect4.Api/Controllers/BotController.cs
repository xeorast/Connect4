using Connect4.Domain.Dtos;
using Connect4.Engine;
using Microsoft.AspNetCore.Mvc;

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

		try
		{
			return bot.GetRecommendation( well );
		}
		catch ( InvalidOperationException e )
		{
			return BadRequest( e.Message );
		}
	}
}
