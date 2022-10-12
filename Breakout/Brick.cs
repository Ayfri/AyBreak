using System.Windows.Forms;

namespace Breakout;

public class Brick : PictureBox {
	public const int BrickWidth = 50;
	public const int BrickHeight = 20;
	public const int BrickMargin = 5;

	public readonly BrickType Type;

	public Brick(BrickType type) {
		Type = type;
		Tag = new[] { "Brick" };
		Width = BrickWidth;
		Height = BrickHeight;
		BackColor = type.Color;
	}
}
