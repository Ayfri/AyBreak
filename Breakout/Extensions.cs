namespace Breakout;

using System.Windows.Forms;

/// <summary>
///     The Extensions static class contains extension methods for Controls.
/// </summary>
public static class Extensions {
	/// <summary>
	///     The CenterX extension method returns the horizontal center of the control.
	/// </summary>
	/// <param name="control"> The control to get the horizontal center of. </param>
	/// <returns> The horizontal center of the control. </returns>
	public static int CenterX(this Control control) => control.Left + control.Width / 2;

	/// <summary>
	///     The CenterY extension method returns the vertical center of the control.
	/// </summary>
	/// <param name="control"> The control to get the vertical center of. </param>
	/// <returns> The vertical center of the control. </returns>
	public static int CenterY(this Control control) => control.Top + control.Height / 2;
}
