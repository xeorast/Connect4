﻿using Connect4.Domain.Dtos;
using System.Net;
using System.Net.Http.Json;

namespace Connect4.Api.Client;

public partial class C4ApiConsumer
{
	public class C4ApiConsumer_Multiplayer : C4ApiConsumer_Child
	{
		internal C4ApiConsumer_Multiplayer( C4ApiConsumer api ) : base( api ) { }

		public async Task<GameDto?> GetBoard( Guid uuid )
		{
			var response = await Http.GetAsync( $"/api/multiplayer/{uuid}" ).ConfigureAwait( false );

			return response.StatusCode switch
			{
				HttpStatusCode.OK => await response.Content.ReadFromJsonAsync<GameDto>().ConfigureAwait( false ),
				HttpStatusCode.NotFound => null,
				_ => throw response.CodeException(),
			};
		}

		public async Task<Guid> CreateGame()
		{
			var response = await Http.PostAsync( "api/multiplayer", null ).ConfigureAwait( false );

			return response.StatusCode switch
			{
				HttpStatusCode.Created => await response.Content.ReadFromJsonAsync<Guid>().ConfigureAwait( false ),
				_ => throw response.CodeException(),
			};
		}

		public async Task Move( Guid uuid, int column )
		{
			var response = await Http.PostAsJsonAsync( $"api/multiplayer/{uuid}", column ).ConfigureAwait( false );

			if ( response.StatusCode is HttpStatusCode.OK )
			{
				return;
			}
			throw response.CodeException( await response.Content.ReadAsStringAsync().ConfigureAwait( false ) );
		}

	}

}
