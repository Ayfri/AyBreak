namespace Breakout.Entities;

using System;
using System.Drawing;
using System.Windows.Forms;

public sealed class Brick : PictureBox {
	/// <summary>
	///     The Brick's Width constant.
	/// </summary>
	public const int BrickWidth = 50;

	/// <summary>
	///     The Brick's Height constant.
	/// </summary>
	public const int BrickHeight = 20;

	/// <summary>
	///     The Brick's Margin constant.
	/// </summary>
	public const int BrickMargin = 1;

	/// <summary>
	///     A Random object used to generate random numbers.
	/// </summary>
	private static readonly Random Random = new();

	/// <summary>
	///     The Brick's label, which displays the Brick's value for debugging purposes.
	/// </summary>
	private readonly Label? _label;

	/// <summary>
	///     The type of Brick this is.
	/// </summary>
	public readonly BrickType Type;

	/// <summary>
	///     The health of the Brick.
	/// </summary>
	private int _health;

	/// <summary> The Brick function creates a new brick object. </summary>
	/// <param name="type"> The type of brick to create. </param>
	public Brick(BrickType type) {
		Type = type;
		Width = BrickWidth;
		Height = BrickHeight;
		BackColor = BrickType.GetColor(type, Health);
		Health = type.MaxHealth;

		#if DEBUG
		_label = new() {
			AutoSize = true,
			Font = new(Program.MainFont, 8),
			ForeColor = Color.White,
			Location = new(0, 0),
			Text = Health.ToString()
		};

		Controls.Add(_label);
		#endif
	}

	/// <summary>
	///     The Health property gets or sets the health of the brick and updates the color and label.
	/// </summary>
	public int Health {
		get => _health;
		set {
			_health = value;
			BackColor = BrickType.GetColor(Type, _health);
			if (_label != null) _label.Text = _health.ToString();
		}
	}

	/// <summary>
	///     The Hit function is called when the brick is hit by the ball.
	/// </summary>
	/// <param name="game"> The game object. </param>
	/// <param name="ball"> The ball object. </param>
	/// <param name="touchedSide"> The side of the brick that was hit. </param>
	/// <returns> True if the brick was destroyed, false otherwise. </returns>
	public bool Hit(GameScene game, Ball ball, Side touchedSide) {
		var collisionPayload = new CollisionPayload {
			Game = game,
			Brick = this,
			Ball = ball,
			Side = touchedSide,
			BrickPosition = game.GetBrickPos(this)
		};

		if (!Type.ValidateHit?.Invoke(collisionPayload) ?? false) return false;
		Health--;
		if (Health > 0) return false;

		game.RemoveBrick(this);
		Type.OnCollision?.Invoke(collisionPayload);

		if (Random.NextDouble() > .1) return true;

		var powerUp = PowerUp.GeneratePowerUp(game);
		powerUp.Location = new(Location.X + Width / 2, Location.Y + Height);
		game.AddPowerUp(powerUp);

		return true;
	}
}