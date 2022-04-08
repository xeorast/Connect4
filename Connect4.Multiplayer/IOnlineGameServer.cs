namespace Connect4.Multiplayer;

public interface IOnlineGameServer
{
	Task Move( int column );
}
