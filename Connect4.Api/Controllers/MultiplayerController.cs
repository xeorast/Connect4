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
		var gameDto = await _multiplayerService.GetBoardAsync( uuid );
		if ( gameDto is null )
		{
			return NotFound();
		}

		return gameDto;
	}

	[HttpPost]
	public async Task<ActionResult<Guid>> CreateGame()
	{
		var uuid = await _multiplayerService.CreateGameAsync();

		return CreatedAtAction( nameof( GetBoard ), new { uuid }, uuid );
	}

	[HttpPost( "{uuid}/move" )]
	public async Task<IActionResult> Move( Guid uuid, [FromBody] int column )
	{
		try
		{
			_ = await _multiplayerService.MoveAsync( uuid, column );
		}
		catch ( NotFoundException e )
		{
			return NotFound( e.Message );
		}
		catch ( InvalidOperationException e )
		{
			return BadRequest( e.Message );
		}
		catch ( ArgumentOutOfRangeException e )
		{
			return BadRequest( e.Message );
		}
		return Ok();
	}

}
