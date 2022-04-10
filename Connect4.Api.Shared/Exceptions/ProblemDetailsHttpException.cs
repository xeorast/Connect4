using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Text.Json;

namespace Connect4.Api.Shared.Exceptions;


[Serializable]
public class ProblemDetailsHttpException : HttpRequestException
{
	public ProblemDetails ProblemDetails { get; set; }
	public ProblemDetailsHttpException( ProblemDetails problemDetails )
		: this( problemDetails, (HttpStatusCode?)problemDetails.Status ) { }
	public ProblemDetailsHttpException( ProblemDetails problemDetails, HttpStatusCode? statusCode )
		: base( JsonSerializer.Serialize( problemDetails ), null, statusCode )
	{
		ProblemDetails = problemDetails;
	}
}
