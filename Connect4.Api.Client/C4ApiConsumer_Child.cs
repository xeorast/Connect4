namespace Connect4.Api.Client;

public partial class C4ApiConsumer
{
	public abstract class C4ApiConsumer_Child
	{
		protected C4ApiConsumer Api { get; }
		protected HttpClient Http => Api.Http;
		protected C4ApiConsumer_Child( C4ApiConsumer api ) => Api = api;
	}

}
