namespace Breakout;

using System.Drawing;
using System.Windows.Forms;

public sealed class ScoreLabel : Label {
	private const float Speed = .4f;

	private readonly int _maxY;

	public ScoreLabel(Point initialPosition, int maxY, int score) {
		_maxY = maxY;
		Location = initialPosition;
		BackColor = Color.Transparent;
		ForeColor = Color.White;
		Text = $"+{score}";
		AutoSize = true;
		FontHeight = 20;
	}

	public new bool Move(int deltaTime) {
		Top -= (int)(Speed * deltaTime);
		// change opacity from 256 to 0
		ForeColor = Color.FromArgb(256 - (Top - _maxY) * 256 / (Location.Y - _maxY), ForeColor);
		return Top <= _maxY;
	}
}