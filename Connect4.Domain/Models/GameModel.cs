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
		//var state = Convert.FromBase64String( State );

		//Hue[,] board = new Hue[Width, Height];

		//for ( int col = 0; col < Width; col++ )
		//{
		//	for ( int row = 0; row < Height; row++ )
		//	{
		//		board[col, row] = (Hue)state[row * Width + col];
		//	}
		//}

		//Well well = new( board, ToConnect );

		//return new(well, )

		return Game.DeserializeFromJson( State ) ?? throw new NullReferenceException( "game was null" );
	}

	public void UpdateState( Game game )
	{
		State = game.SerializeToJson();
	}

}
