namespace Breakout.Entities;

using System.Windows.Forms;

public delegate void MouseMovedEvent(MouseEventArgs e);

public class GlobalMouseHandler : IMessageFilter {
	private const int WmMousemove = 0x0200;

	public event MouseMovedEvent? MouseMovedEvent;

	#region IMessageFilter Members

	public bool PreFilterMessage(ref Message m) {
		if (m.Msg != WmMousemove) return false;
		var e = new MouseEventArgs(MouseButtons.None, 0, Cursor.Position.X, Cursor.Position.Y, 0);
		MouseMovedEvent?.Invoke(e);
		return false;
	}

	#endregion
}
