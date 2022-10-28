namespace Breakout;

using System.Drawing;
using Breakout.Entities;

/// <summary>
///     A class for storing data when a collision occurs.
/// </summary>
public sealed class CollisionPayload {
	/// <summary>
	///     The ball that collided with the entity.
	/// </summary>
	public Ball Ball;

	/// <summary>
	///     The brick that was hit.
	/// </summary>
	public Brick Brick;

	/// <summary>
	///     The brick position in the grid.
	/// </summary>
	public Point BrickPosition;

	/// <summary>
	///     The game.
	/// </summary>
	public GameScene Game = null!;

	/// <summary>
	///     The side of the brick that was hit.
	/// </summary>
	public Side Side;
}