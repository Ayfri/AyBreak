using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Breakout.Entities;

public class Brick : PictureBox {
	public const int BrickWidth = 50;
	public const int BrickHeight = 20;
	public const int BrickMargin = 1;
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

	public void Hit(GameScene game, Side touchedSide) {
		Health--;
		if (Health > 0) return;
		game.RemoveBrick(this);

		var collisionPayload = new OnCollision {
			Game = game,
			Brick = this,
			Ball = game.Ball,
			Side = touchedSide,
			BrickPosition = game.GetBrickPos(this)
		};

		Type.OnCollision?.Invoke(collisionPayload);
	}
}
