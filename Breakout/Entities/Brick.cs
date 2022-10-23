namespace Breakout.Entities;

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

public sealed class Brick : PictureBox {
	public const int BrickWidth = 50;
	public const int BrickHeight = 20;
	public const int BrickMargin = 1;
	private static readonly Random Random = new();
	private readonly Label? _label;
	public readonly BrickType Type;

	private int _health;

	public Brick(BrickType type) {
		Type = type;
		Tag = new[] { "Brick" };
		Width = BrickWidth;
		Height = BrickHeight;
		BackColor = type.GetColor(Health);
		Health = type.MaxHealth;

		_label = new() {
			AutoSize = true,
			Font = new("Arial", 8),
			ForeColor = Color.White,
			Location = new(0, 0),
			Tag = new[] { "BrickHealth" },
			Text = Health.ToString()
		};

		if (Debugger.IsAttached) Controls.Add(_label);
	}

	public int Health {
		get => _health;
		set {
			_health = value;
			BackColor = Type.GetColor(_health);
			if (_label != null) _label.Text = _health.ToString();
		}
	}

	public void Hit(GameScene game, Ball ball, Side touchedSide) {
		Health--;
		if (Health > 0) return;
		game.RemoveBrick(this);

		var collisionPayload = new CollisionPayload {
			Game = game,
			Brick = this,
			Ball = ball,
			Side = touchedSide,
			BrickPosition = game.GetBrickPos(this)
		};

		Type.OnCollision?.Invoke(collisionPayload);

		if (!(Random.NextDouble() > .9)) return;

		var powerUp = GeneratePowerUp(game);
		powerUp.Location = new(Location.X + Width / 2, Location.Y + Height);
		game.AddPowerUp(powerUp);
	}

	private static PowerUp GeneratePowerUp(GameScene game) {
		var powerUpCount = typeof(PowerUpType).GetEnumValues().Length;

		PowerUpType powerUpType;
		bool invalid;

		do {
			powerUpType = (PowerUpType) Random.Next(powerUpCount);

			invalid = powerUpType switch {
				PowerUpType.BallNoClip => game.IsNoClip,
				PowerUpType.BallSpeedUp => game.BallSpeedMultiplier >= 2,
				PowerUpType.IncreasePaddleSize => game.Paddle.Width >= 300,
				PowerUpType.MoreBall => game.BallCount >= 4,
				PowerUpType.MoreLife => false,
				PowerUpType.ScoreMultiplier => false,
				_ => throw new ArgumentOutOfRangeException()
			};
		} while (invalid);

		var value = powerUpType switch {
			PowerUpType.BallNoClip => 1,
			PowerUpType.BallSpeedUp => Random.NextDouble() * .1,
			PowerUpType.IncreasePaddleSize => Random.Next(10, 50),
			PowerUpType.MoreBall => 1,
			PowerUpType.MoreLife => 1,
			PowerUpType.ScoreMultiplier => Random.NextDouble() * 2,
			_ => throw new ArgumentOutOfRangeException()
		};

		var powerUp = new PowerUp(powerUpType, value);
		return powerUp;
	}
}