namespace Breakout;

using System;
using System.Drawing;
using System.Windows.Forms;

public sealed class PauseMenu : Panel {
	public PauseMenu() {
		BackColor = Color.FromArgb(100, 40, 40, 40);
		Size = new(600, 450);

		var label = new Label {
			AutoSize = true,
			Font = new(Program.MainFont, 40),
			ForeColor = Color.White,
			Text = "Paused",
			Width = 40 * 7
		};

		var textMeasure = TextRenderer.MeasureText(label.Text, label.Font);
		label.Location = new((Width - textMeasure.Width) / 2, 50);

		var resumeButton = new PauseMenuButton(
			"Resume",
			(_, _) => { (Parent as GameScene)?.HidePauseMenu(); }
		);

		resumeButton.Location = new((Width - resumeButton.Width) / 2, (Height - resumeButton.Height) / 2);

		var restartButton = new PauseMenuButton(
			"Restart",
			(_, _) => {
				var level = (Parent as GameScene)?.Level;

				if (level == null) Program.MainForm.ChangeScene(new LevelSelectionScene());
				else Program.MainForm.ChangeScene(new GameScene(level.Value));
			}
		);

		restartButton.Location = new((Width - restartButton.Width) / 2, (Height - restartButton.Height) / 2 + 75);

		var exitButton = new PauseMenuButton(
			"Quit",
			static (_, _) => Program.MainForm.ChangeScene(new MainMenuScene())
		);

		exitButton.Location = new((Width - exitButton.Width) / 2, (Height - exitButton.Height) / 2 + 150);

		Controls.Add(label);
		Controls.Add(resumeButton);
		Controls.Add(restartButton);
		Controls.Add(exitButton);
	}

	private sealed class PauseMenuButton : Button {
		public PauseMenuButton(string text, EventHandler clickHandler) {
			Text = text;
			Click += clickHandler;
			Width = 200;
			Height = 50;
			BackColor = Color.FromArgb(30, 30, 30);
			ForeColor = Color.White;
			FlatStyle = FlatStyle.Flat;
			FlatAppearance.BorderSize = 0;
			Font = new(Program.MainFont, 20);
		}
	}
}
