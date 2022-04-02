using Connect4.Api.Exceptions;
using Connect4.Api.Services;
using Connect4.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Connect4.Api.Controllers;

[ApiController]
[Route( "api/[controller]" )]
public class MultiplayerController : ControllerBase
{
	private readonly IMultiplayerService _multiplayerService;

	public MultiplayerController( IMultiplayerService multiplayerService )
	{
		_multiplayerService = multiplayerService;
	}

	[HttpGet( "{uuid}" )]
	public async Task<ActionResult<GameDto>> GetBoard( Guid uuid )
	{
		var model = await _multiplayerService.GetBoardAsync( uuid );
		if ( model is null )
		{
			return NotFound();
		}

		var game = model.GetGameFromState();
		var well = game.CloneWell();

		WellDto wellDto = new( well.ToConnect, well.WellObj );
		GameDto gameDto = new( game.NumberPlayers, game.Winner, game.CurrentPlayer, wellDto );

		return gameDto;
	}

	[HttpPost]
	public async Task<ActionResult<Guid>> CreateGame()
	{
		return Ok( await _multiplayerService.CreateGameAsync() );
	}

	[HttpPost( "{uuid}/move" )]
	public async Task<IActionResult> Move( Guid uuid, [FromBody] int column )
	{
		try
		{
			await _multiplayerService.MoveAsync( uuid, column );
			return Ok();
		}
		catch ( NotFoundException e )
		{
			return NotFound( e.Message );
		}
	}

}
