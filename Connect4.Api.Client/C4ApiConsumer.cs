namespace Connect4.Api.Client;

internal static class ApiConsumerHelpers
{
	public static HttpRequestException CodeException( this HttpResponseMessage response )
	{
		return new HttpRequestException( "unexpected status code", null, response.StatusCode );
	}
	public static HttpRequestException CodeException( this HttpResponseMessage response, string message )
	{
		return new HttpRequestException( message, null, response.StatusCode );
	}
}

public partial class C4ApiConsumer
{
	public Uri BaseAddress { get; }
	private HttpClient Http => httpClient.Value;
	private readonly Lazy<HttpClient> httpClient;

	public C4ApiConsumer_Multiplayer Multiplayer => multiplayer.Value;
	private readonly Lazy<C4ApiConsumer_Multiplayer> multiplayer;
	public C4ApiConsumer_Multiplayer_RealTime RealTimeMultiplayer => realTimeMultiplayer.Value;
	private readonly Lazy<C4ApiConsumer_Multiplayer_RealTime> realTimeMultiplayer;

	public C4ApiConsumer( string baseAddress ) : this( new Uri( baseAddress ) ) { }
	public C4ApiConsumer( Uri baseAddress )
	{
		BaseAddress = baseAddress;
		httpClient = new( () => new() { BaseAddress = new( "https://localhost:7126" ) } );
		multiplayer = new( () => new( this ) );
		realTimeMultiplayer = new( () => new( this ) );
	}

	public abstract class C4ApiConsumer_Child
	{
		protected C4ApiConsumer Api { get; }
		protected HttpClient Http => Api.Http;
		protected C4ApiConsumer_Child( C4ApiConsumer api ) => Api = api;
	}

}
