using System.Windows.Forms;

namespace Breakout;

public class AbstractScene : Panel {
	public virtual new void KeyDown(KeyEventArgs e) {}
	public virtual new void KeyUp(KeyEventArgs e) {}
}
