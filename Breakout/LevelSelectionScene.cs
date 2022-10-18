using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Breakout;

public sealed class LevelSelectionScene : AbstractScene {
	private readonly int _levelCount = LevelManager.Levels.Length;

	private sealed class LevelButton : Button {
		public const int BtnWidth = 300;
		public const int BtnHeight = 120;
		public const int BtnMargin = 20;
		public LevelButton(int level) {
			Text = $"Level {level + 1}";
			Width = BtnWidth;
			Height = BtnHeight;
			Font = new("Candara", 30);
			BackColor = Color.FromArgb(20, 20, 25);
			ForeColor = Color.White;

			Click += (_, _) => {
				var layout = LevelManager.LoadLevel(level);
				Program.MainForm.ChangeScene(new GameScene(layout));
			};
		}
	}

	public LevelSelectionScene() {
		BackColor = Color.FromArgb(20, 20, 25);
		Size = Program.MainForm.Size;
		var subtitle = new Label {
			Text = "Select a level",
			Width = 350,
			Height = 100,
			Font = new("Candara", 40),
			ForeColor = Color.White,
		};
		subtitle.Location = new(ClientSize.Width / 2 - subtitle.Width / 2, ClientSize.Height / 6);
		
		Controls.Add(subtitle);

		// set levels in grid of 5x3
		const int totalWidth = LevelButton.BtnWidth * 5 + LevelButton.BtnMargin * 4;
		for (var i = 0; i < _levelCount; i++) {
			var button = new LevelButton(i);
			var x = i % 5 * (LevelButton.BtnWidth + LevelButton.BtnMargin);
			var y = i / 5 * (LevelButton.BtnHeight + LevelButton.BtnMargin);
			button.Location = new(ClientSize.Width / 2 - totalWidth / 2 + x, ClientSize.Height / 2 - 150 + y);
			Controls.Add(button);
		}
	}
}
