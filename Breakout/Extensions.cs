namespace Breakout;

using System.Drawing;
using System.Windows.Forms;

public static class Extensions {
	public static int CenterX(this Rectangle rect) => rect.X + rect.Width / 2;
	public static int CenterY(this Rectangle rect) => rect.Y + rect.Height / 2;

	public static int CenterX(this Control control) => control.Left + control.Width / 2;
	public static int CenterY(this Control control) => control.Top + control.Height / 2;
}
