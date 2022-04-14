namespace Connect4.Domain.Core.GameWrappers;

public struct Coordinate
{
	public int Column { get; set; }
	public int Row { get; set; }

	public static explicit operator (int column, int row)( Coordinate c )
	{
		return (c.Column, c.Row);
	}
	public static explicit operator Coordinate( (int column, int row) s )
	{
		return new Coordinate { Column = s.column, Row = s.row };
	}
}
