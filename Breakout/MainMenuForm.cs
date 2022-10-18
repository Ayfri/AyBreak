using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Breakout;

public class MainMenuForm : AbstractScene {
	private class MenuButton : Button {
		public MenuButton(string text, int offsetY, EventHandler onClick) {
			Text = text;
			Size = new(200, 80);
			Location = new(MainForm.GameSize.Width / 2 - Width / 2, MainForm.GameSize.Height / 2 - Height / 2 + offsetY);
			Font = new("Arial", 20);
			BackColor = Color.FromArgb(20, 20, 30);
			ForeColor = Color.FromArgb(255, 255, 255);
			FlatStyle = FlatStyle.Flat;
			FlatAppearance.BorderSize = 1;
			FlatAppearance.BorderColor = Color.FromArgb(255, 255, 255);
			var p = new GraphicsPath();
			p.AddEllipse(1, 1, Width - 4, Height - 4);
			Region = new(p);
			Click += onClick;
		}
	}

	private MenuButton _exitButton = new(
		"Exit",
		100,
		static (_, _) => {
			Application.Exit();
		}
	);

	private MenuButton _startButton = new("Start", -100, static (_, _) => {
		Program.MainForm.ChangeScene(new GameForm());
	});
	
	public MainMenuForm() {
		BackColor = Color.FromArgb(15, 15, 20);
		Size = MainForm.GameSize;
		Controls.Add(_exitButton);
		Controls.Add(_startButton);
	}
}
