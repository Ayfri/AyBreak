using System.Drawing;
using System.Windows.Forms;

namespace Breakout;

public static class Extensions {
	public static int CenterX(this Rectangle rect) => rect.X + rect.Width / 2;
	public static int CenterY(this Rectangle rect) => rect.Y + rect.Height / 2;

	public static int CenterX(this Control control) => control.Left + control.Width / 2;
	public static int CenterY(this Control control) => control.Top + control.Height / 2;

	// public static Side? GetTouchingSide(this Rectangle rect, Rectangle other) {
	// 	if (rect.CenterY() <= other.CenterY() - other.Height / 2) return Side.Top;
	// 	if (rect.CenterY() >= other.CenterY() + other.Height / 2) return Side.Bottom;
	// 	if (rect.CenterX() > other.CenterX()) return Side.Right;
	// 	if (rect.CenterX() < other.CenterX()) return Side.Left;
	// 	return null;
	// }
}
