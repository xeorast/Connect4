using Connect4.Api.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Connect4.Api.Client;

internal static class HubErrorHelper
{
	static readonly Regex messageRegex = new( @"^An unexpected error occurred invoking '(?<method>\S+)' on the server\. (?<exception>\S+): (?<message>.*)$", RegexOptions.Singleline );
	public static string? GetErorMessage( this HubException exception )
	{
		var match = messageRegex.Match( exception.Message );
		if ( !match.Success )
		{
			return null;
		}
		return match.Groups["message"].Value;
	}

	public static ProblemDetails? GetProblemDetails( this HubException exception )
	{
		var match = messageRegex.Match( exception.Message );
		if ( !match.Success || match.Groups["exception"].Value != nameof( ProblemDetailsHubException ) )
		{
			return null;
		}

		return JsonSerializer.Deserialize<ProblemDetails>( match.Groups["message"].Value );
	}

	public static bool TryGetErorMessage( this HubException exception, [NotNullWhen( true )] out string? message )
	{
		message = exception.GetErorMessage();
		return message is not null;
	}

	public static bool TryGetProblemDetails( this HubException exception, [NotNullWhen( true )] out ProblemDetails? problemDetails )
	{
		problemDetails = exception.GetProblemDetails();
		return problemDetails is not null;
	}

}
