using Connect4.Domain.Dtos.GameEvents;

namespace Connect4.Multiplayer;

public interface IOnlineGameClient
{
	Task PlayerMoved( PlayerMovedDto d );
	Task GameEnded( GameEndedDto d );
	Task ColumnFilled( ColumnFilledDto d );
	Task PlayerSwitched( PlayerSwitchedDto d );
	Task TurnCompleted();
}
