using System;
using System.Windows.Forms;

namespace Breakout;

internal static class Program {
	public static MainForm MainForm = null!;

	[STAThread]
	private static void Main() {
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		MainForm = new();
		Application.Run(MainForm);
	}
}
