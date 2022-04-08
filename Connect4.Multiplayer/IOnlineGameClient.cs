using Connect4.Domain.Dtos.GameEvents;

namespace Connect4.Multiplayer;

public interface IOnlineGameClient
{
	Task PlayerMoved( PlayerMovedDto d );
	delegate Task PlayerMovedHandler( PlayerMovedDto d );
	Task GameEnded( GameEndedDto d );
	delegate Task GameEndedHandler( GameEndedDto d );
	Task ColumnFilled( ColumnFilledDto d );
	delegate Task ColumnFilledHandler( ColumnFilledDto d );
	Task PlayerSwitched( PlayerSwitchedDto d );
	delegate Task PlayerSwitchedHandler( PlayerSwitchedDto d );
	Task TurnCompleted();
	delegate Task TurnCompletedHandler();
}
