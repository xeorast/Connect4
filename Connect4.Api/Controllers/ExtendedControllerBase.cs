using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Connect4.Api.Controllers;

public class ExtendedControllerBase : ControllerBase
{
	[NonAction]
	private ProblemDetails CreateProblemDetails( int? statusCode = null, string? title = null, string? type = null, string? detail = null, string? instance = null )
	{
		if ( ProblemDetailsFactory is not null )
		{
			return ProblemDetailsFactory.CreateProblemDetails( HttpContext, statusCode, title, type, detail, instance );
		}

		return new ProblemDetails
		{
			Detail = detail,
			Instance = instance,
			Status = statusCode ?? StatusCodes.Status500InternalServerError,
			Title = title,
			Type = type,
		};
	}

	[NonAction]
	public NotFoundObjectResult NotFound( string detail )
	{
		var pd = CreateProblemDetails(
			statusCode: StatusCodes.Status404NotFound,
			detail: detail );

		return NotFound( pd );
	}

	[NonAction]
	public BadRequestObjectResult BadRequest( string detail )
	{
		var pd = CreateProblemDetails(
			statusCode: StatusCodes.Status400BadRequest,
			detail: detail );

		return BadRequest( pd );
	}

}
