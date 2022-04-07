using Connect4.Domain.Core;
using Connect4.Domain.JsonConverters;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Connect4.Domain.Models;

[Index( nameof( Uuid ) )]
public class GameModel
{
	public GameModel( Guid uuid, string wellState )
	{
		Uuid = uuid;
		WellState = wellState;
	}
	public GameModel( Guid uuid, Game game )
	{
		Uuid = uuid;
		NumberPlayers = game.NumberPlayers;
		Winner = game.Winner;
		CurrentPlayer = game.CurrentPlayer;

		var well = game.CloneWell();
		ToConnect = well.ToConnect;
		Well = well.WellObj;
	}

	[Key]
	public int Id { get; set; }
	public Guid Uuid { get; set; }

	public int NumberPlayers { get; set; }
	public Hue? Winner { get; set; }
	public Hue CurrentPlayer { get; set; }

	public int ToConnect { get; set; }
	[NotMapped]
	public Hue[,] Well { get; set; }
	[MemberNotNull( nameof( Well ) )]
	[EditorBrowsable( EditorBrowsableState.Never )]
	public string WellState
	{
		get
		{
			return JsonSerializer.Serialize( Well ?? throw new NullReferenceException( "game was null" ), Array2DJsonConverter<Hue>.OptionsWithConverter );
		}
		set
		{
			Well = JsonSerializer.Deserialize<Hue[,]>( value, Array2DJsonConverter<Hue>.OptionsWithConverter )
				?? throw new NullReferenceException( "game was null" );
		}
	}

	[NotMapped]
	private Game? GameInstance { get; set; }
	private readonly object GameInstanceLock = new();
	public Game GetGameFromState()
	{
		lock ( GameInstanceLock )
		{
			GameInstance ??= new Game(
				NumberPlayers,
				Winner,
				CurrentPlayer,
				new Well( Well, ToConnect )
				);

			return GameInstance; 
		}
	}

	public void UpdateState( Game game )
	{
		NumberPlayers = game.NumberPlayers;
		Winner = game.Winner;
		CurrentPlayer = game.CurrentPlayer;

		var well = game.CloneWell();
		ToConnect = well.ToConnect;
		Well = well.WellObj;
	}

}
