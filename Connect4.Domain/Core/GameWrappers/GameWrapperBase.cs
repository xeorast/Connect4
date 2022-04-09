using Connect4.Domain.Dtos.GameEvents;

namespace Connect4.Domain.Core.GameWrappers;

public struct Coordinate
{
	public int Column { get; set; }
	public int Row { get; set; }
}

public abstract class GameWrapperBase
{
	public Guid Uuid { get; protected set; }
	public abstract Hue this[Coordinate cord] { get; }

	public int Columns { get; protected set; }
	public int Rows { get; protected set; }
	public int ToConnect { get; protected set; }

	public virtual bool HasEnded => Winner is not null;
	public Hue StartingPlayer { get; protected set; }
	public abstract Hue CurrentPlayer { get; }
	public abstract Hue? Winner { get; }
	public Coordinate? PreviousMove { get; private set; }

	public enum PlayerType
	{
		Player,
		Computer
	}
	public Dictionary<Hue, PlayerType> Players { get; } = new();
	public enum GameMode
	{
		PvP,
		PvC,
		CvC
	}

	public bool IsNowPlayer => Players[CurrentPlayer] is PlayerType.Player;
	public bool IsNowBot => Players[CurrentPlayer] is PlayerType.Computer;
	public abstract bool CanMove { get; }

	public event EventHandler<PlayerMovedDto>? PlayerMoved;
	public event EventHandler<GameEndedDto>? GameEnded;
	public event EventHandler<ColumnFilledDto>? ColumnFilled;
	public event EventHandler<PlayerSwitchedDto>? PlayerSwitched;
	public event EventHandler? TurnCompleted;
	protected void InvokePlayerMoved( PlayerMovedDto d )
	{
		PlayerMoved?.Invoke( this, d );
		PreviousMove = new() { Column = d.Column, Row = d.Row };
	}
	protected void InvokeGameEnded( GameEndedDto d ) => GameEnded?.Invoke( this, d );
	protected void InvokeColumnFilled( ColumnFilledDto d ) => ColumnFilled?.Invoke( this, d );
	protected void InvokePlayerSwitched( PlayerSwitchedDto d ) => PlayerSwitched?.Invoke( this, d );
	protected void InvokeTurnCompleted() => TurnCompleted?.Invoke( this, EventArgs.Empty );

	public abstract void Move( int column );
	public abstract void MoveBot( TimeSpan minMoveTime );
	public abstract IEnumerable<Coordinate> GetWinning();

}
