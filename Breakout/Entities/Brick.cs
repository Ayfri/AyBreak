namespace Breakout.Entities;

using System;
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

		Tag = new[] {
			"Brick"
		};

		Width = BrickWidth;
		Height = BrickHeight;
		BackColor = type.GetColor(Health);
		Health = type.MaxHealth;
		#if DEBUG
		_label = new() {
			AutoSize = true,
			Font = new(Program.MainFont, 8),
			ForeColor = Color.White,
			Location = new(0, 0),
			Tag = new[] {
				"BrickHealth"
			},
			Text = Health.ToString()
		};

		Controls.Add(_label);
		#endif
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

		if (Random.NextDouble() > .1) return;

		var powerUp = PowerUp.GeneratePowerUp(game);
		powerUp.Location = new(Location.X + Width / 2, Location.Y + Height);
		game.AddPowerUp(powerUp);
	}
}
