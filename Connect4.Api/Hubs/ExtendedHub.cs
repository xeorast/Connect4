using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.SignalR;

namespace Connect4.Api.Hubs;

public class ExtendedHub<T> : Hub<T> where T : class
{
	public ProblemDetailsFactory? ProblemDetailsFactory
	{
		get => _problemDetailsFactory ??= HttpContext?.RequestServices?.GetRequiredService<ProblemDetailsFactory>();
		set => _problemDetailsFactory = value ?? throw new ArgumentNullException( nameof( value ) );
	}
	private ProblemDetailsFactory? _problemDetailsFactory;

	public HttpContext? HttpContext
	{
		get => _httpContext ??= Context.GetHttpContext();
	}
	private HttpContext? _httpContext;

	private ProblemDetails CreateProblemDetails( int? statusCode = null, string? title = null, string? type = null, string? detail = null, string? instance = null )
	{
		if ( this is { HttpContext: not null, ProblemDetailsFactory: not null } )
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

	public ProblemDetails NotFound( string detail ) => CreateProblemDetails( statusCode: StatusCodes.Status404NotFound, detail: detail );
	public ProblemDetails BadRequest( string detail ) => CreateProblemDetails( statusCode: StatusCodes.Status400BadRequest, detail: detail );

}
