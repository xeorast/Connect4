using Microsoft.AspNetCore.SignalR.Client;

namespace Connect4.Api.Client;

public partial class C4ApiConsumer
{
	public abstract class C4ApiConsumer_Child
	{
		protected C4ApiConsumer Api { get; }
		protected HttpClient Http => Api.Http;
		protected C4ApiConsumer_Child( C4ApiConsumer api ) => Api = api;
	}
	public abstract class C4ApiConsumer_RealTimeChild : IAsyncDisposable
	{
		/*
		 * if reference to ApiConsumer is removed without stoping connection, 
		 * having normal reference to it would stop GC from collecting it
		 * resulting in zombie connection. Weak reference allows GC to collect 
		 * it and call Disconnect in its destructor.
		 */
		protected C4ApiConsumer Api
		{
			get
			{
				if ( api.TryGetTarget( out var ret ) )
				{
					return ret;
				}
				else
				{
					DisposeAsync().AsTask().ConfigureAwait( false ).GetAwaiter().GetResult();
					return null!;
				}
			}
		}
		protected WeakReference<C4ApiConsumer> api;
		//protected C4ApiConsumer Api => api;
		//protected C4ApiConsumer api;
		protected HttpClient Http => Api.Http;
		protected HubConnection HubConnection => hubConnection.Value;
		private readonly Lazy<HubConnection> hubConnection;

		protected C4ApiConsumer_RealTimeChild( C4ApiConsumer api )
		{
			this.api = new( api );
			//this.api = api;
			hubConnection = new( ConfigureConnection );
		}

		protected abstract HubConnection ConfigureConnection();

		public abstract Task ConnectAsync();
		public abstract Task DisconnectAsync();
		//public Task<bool> TryDisconnectAsync()
		//{
		//	if ( hubConnection is { IsValueCreated: true, Value.State: not HubConnectionState.Disconnected } )
		//	{
		//		await DisconnectAsync().ConfigureAwait( false );
		//		return true;
		//	}
		//	return false;
		//}

		protected virtual ValueTask DisposeAsyncInteral()
		{
			return ValueTask.CompletedTask;
		}

		public async ValueTask DisposeAsync()
		{
			await DisposeAsyncInteral().ConfigureAwait( false );

			if ( hubConnection is { IsValueCreated: true, Value: var h} )
			{
				await ( (IAsyncDisposable)h ).DisposeAsync().ConfigureAwait( false );
			}
			GC.SuppressFinalize( this );
		}

	}

}
