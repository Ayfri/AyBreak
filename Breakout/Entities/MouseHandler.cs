namespace Breakout.Entities;

using System.Windows.Forms;

/// <summary>
///     The delegate for the event that is raised when the mouse moves on the screen.
/// </summary>
public delegate void MouseMovedEvent(MouseEventArgs e);

/// <summary>
///     A handler for the mouse moved event;
/// </summary>
public sealed class GlobalMouseHandler : IMessageFilter {
	/// <summary>
	///     The constant for the mouse move message.
	/// </summary>
	private const int WmMousemove = 0x0200;

	/// <summary>
	///     The method to filter out the messages.
	/// </summary>
	/// <param name="m"> The message to filter. </param>
	/// <returns> True if the message was filtered out. </returns>
	public bool PreFilterMessage(ref Message m) {
		if (m.Msg != WmMousemove) return false;
		var e = new MouseEventArgs(MouseButtons.None, 0, Cursor.Position.X, Cursor.Position.Y, 0);
		MouseMovedEvent?.Invoke(e);
		return false;
	}

	/// <summary>
	///     The event that is raised when the mouse moves.
	/// </summary>
	public event MouseMovedEvent? MouseMovedEvent;
}