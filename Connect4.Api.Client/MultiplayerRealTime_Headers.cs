namespace Connect4.Api.Client;

public partial class C4ApiConsumer
{
	public partial class C4ApiConsumer_Multiplayer_RealTime
	{
		internal class MultiplayerRealTime_Headers : Dictionary<string, string>
		{
			public MultiplayerRealTime_Headers()
			{
				GameId = string.Empty;
				Player = string.Empty;
			}

			public bool IsGameIdSet => !string.IsNullOrEmpty( GameId );
			public string GameId
			{
				get => this["GameId"];
				set => this["GameId"] = value;
			}

			public bool IsPlayerSet => !string.IsNullOrEmpty( Player );
			public string Player
			{
				get => this["Player"];
				set => this["Player"] = value;
			}

		}
	}

}
