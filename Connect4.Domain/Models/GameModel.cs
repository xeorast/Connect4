using Connect4.Engine;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Connect4.Domain.Models;

[Index( nameof( Uuid ) )]
public class GameModel
{
	public GameModel( Guid uuid, string state/*, int width, int height, int toConnect*/ )
	{
		Uuid = uuid;
		State = state;
	}
	public GameModel( Guid uuid, Game game )
	{
		Uuid = uuid;
		State = game.SerializeToJson();
	}

	[Key]
	public int Id { get; set; }
	public Guid Uuid { get; set; }
	public string State { get; set; }

	public Game GetGameFromState()
	{
		return Game.DeserializeFromJson( State )
			?? throw new NullReferenceException( "game was null" );
	}

	public void UpdateState( Game game )
	{
		State = game.SerializeToJson();
	}

}
