using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Breakout;

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

		// if in debug mode, show health
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

	public bool Hit() => --Health == 0;
}
