using Microsoft.AspNetCore.Http.Extensions;

namespace Connect4.Api.Middleware;

public class ProxyHttpsDetectionMiddleware
{
	private const string protocolHeader = "x-forwarded-proto";
	private readonly RequestDelegate _next;

	public ProxyHttpsDetectionMiddleware( RequestDelegate next )
	{
		_next = next;
	}

	public Task Invoke( HttpContext context )
	{
		if ( context.Request.Headers[protocolHeader] != "https" )
		{
			context.Request.IsHttps = true;
		}
		return _next( context );
	}

}
