namespace Breakout;

using System;
using System.Windows.Forms;

/// <summary>
///     The main class for the application.
/// </summary>
public static class Program {
	/// <summary>
	///     The font used for the game.
	/// </summary>
	public const string MainFont = "Candara";

	/// <summary>
	///     The Form used for the game.
	/// </summary>
	public static MainForm MainForm = null!;

	/// <summary> The Main function is the entry point for the application. </summary>
	[STAThread]
	private static void Main() {
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		MainForm = new();
		Application.Run(MainForm);
	}
}
