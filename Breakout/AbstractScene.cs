namespace Breakout;

using System.Windows.Forms;

/// <summary>
///     An Abstract class that represents a Scene as a Panel.
/// </summary>
public class AbstractScene : Panel {
	/// <summary> The KeyDown function is called when a key is pressed. </summary>
	/// <param name="e"> The KeyEventArgs. </param>
	public virtual new void KeyDown(KeyEventArgs e) { }

	/// <summary>
	///     The KeyUp function is called when a key is released.
	/// </summary>
	/// <param name="e"> The KeyEventArgs. </param>
	public virtual new void KeyUp(KeyEventArgs e) { }

	/// <summary>
	///     The MouseMove function is called when the mouse is moved.
	/// </summary>
	/// <param name="e"> The MouseEventArgs. </param>
	public virtual new void MouseMove(MouseEventArgs e) { }
}
