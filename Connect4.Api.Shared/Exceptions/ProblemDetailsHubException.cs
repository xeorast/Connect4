using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace Connect4.Api.Shared.Exceptions;


[Serializable]
public class ProblemDetailsHubException : HubException
{
	public ProblemDetails ProblemDetails { get; set; }
	public ProblemDetailsHubException( ProblemDetails problemDetails )
		: base( JsonSerializer.Serialize( problemDetails ) )
	{
		ProblemDetails = problemDetails;
	}
	protected ProblemDetailsHubException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context ) : base( info, context )
	{
		ProblemDetails = JsonSerializer.Deserialize<ProblemDetails>( Message )!;
	}
}
