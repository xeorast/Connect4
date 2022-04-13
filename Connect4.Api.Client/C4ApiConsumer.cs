using Connect4.Api.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Connect4.Api.Client;

internal static class ApiConsumerHelpers
{
	public static async Task<HttpRequestException> ProblemDetailsOrMessageException( this HttpResponseMessage response )
	{
		var details = await response.Content.ReadFromJsonAsync<ProblemDetails>().ConfigureAwait( false );
		if ( details is not null )
		{
			return new ProblemDetailsHttpException( details );
		}

		var message = await response.Content.ReadAsStringAsync().ConfigureAwait( false );
		if ( message is not null )
		{
			return new HttpRequestException( message, null, response.StatusCode );
		}

		return new HttpRequestException( "unexpected status code", null, response.StatusCode );
	}
	public static async Task<HttpRequestException> ProblemDetailsOrMessageException( this HttpResponseMessage response, string message )
	{
		var details = await response.Content.ReadFromJsonAsync<ProblemDetails>().ConfigureAwait( false );
		if ( details is not null )
		{
			return new ProblemDetailsHttpException( details );
		}

		return new HttpRequestException( message, null, response.StatusCode );
	}

}

public partial class C4ApiConsumer : IAsyncDisposable
{
	public Uri BaseAddress { get; }
	private HttpClient Http => httpClient.Value;
	private readonly Lazy<HttpClient> httpClient;

	public C4ApiConsumer_Multiplayer Multiplayer => multiplayer.Value;
	private readonly Lazy<C4ApiConsumer_Multiplayer> multiplayer;
	public C4ApiConsumer_Multiplayer_RealTime RealTimeMultiplayer => realTimeMultiplayer.Value;
	private readonly Lazy<C4ApiConsumer_Multiplayer_RealTime> realTimeMultiplayer;
	private bool disposedValue;

	public C4ApiConsumer( string baseAddress ) : this( new Uri( baseAddress ) ) { }
	public C4ApiConsumer( Uri baseAddress )
	{
		BaseAddress = baseAddress;
		httpClient = new( () => new() { BaseAddress = new( "https://localhost:7126" ) } );
		multiplayer = new( () => new( this ) );
		realTimeMultiplayer = new( () => new( this ) );
	}

	protected virtual async ValueTask DisposeAsync( bool disposing )
	{
		if ( !disposedValue )
		{
			if ( disposing )
			{
				if ( httpClient.IsValueCreated )
				{
					( (IDisposable)httpClient.Value ).Dispose();
				}
			}

			// treating this as unmanages since it will not dispose itself
			if ( realTimeMultiplayer.IsValueCreated )
			{
				await ( (IAsyncDisposable)realTimeMultiplayer.Value ).DisposeAsync().ConfigureAwait( false );
			}
			disposedValue = true;
		}
	}

	~C4ApiConsumer()
	{
		DisposeAsync( disposing: false ).AsTask().GetAwaiter().GetResult();
	}

	public async ValueTask DisposeAsync()
	{
		await DisposeAsync( disposing: true ).ConfigureAwait( false );
		GC.SuppressFinalize( this );
	}
}
