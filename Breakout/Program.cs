namespace Breakout;

using System;
using System.Windows.Forms;

internal static class Program {
	public const string MainFont = "Candara";
	public static MainForm MainForm = null!;

	[STAThread]
	private static void Main() {
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		MainForm = new();
		Application.Run(MainForm);
	}
}
